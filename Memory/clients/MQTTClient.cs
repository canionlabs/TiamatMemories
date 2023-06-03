using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Memory.utils;
using uPLibrary.Networking.M2Mqtt.Messages;
using MQClient = uPLibrary.Networking.M2Mqtt.MqttClient;

namespace Memory.clients
{
	public class MQTTClient : IClient, IDataResource
	{
		// ========= PUBLIC MEMBERS ===================================================================================

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Memory.clients.MQTTClient"/> is available.
		/// </summary>
		/// <value><c>true</c> if is available; otherwise, <c>false</c>.</value>
		public bool IsAvailable
		{
			get
			{
				return _IsSetup && !_disposedValue;
			}
		}

		public string ClientID { get; private set; }

		public ClientType ClientType => ClientType.MQTT;
		//public event Action<string, string> MessageHandler;

		// ========= PRIVATE MEMBERS ===================================================================================

		private const int CONNECTION_DELAY = 5;

		private MQClient _client;
		private IPEndPoint _brokerEndPoint;

		//private Dictionary<int, string> _subscribedTopicsMapping;
		private Dictionary<string, Action<string, string>> _subscribedHandlerMapping;

		private bool _IsSetup;
		private bool _disposedValue;
		private DateTime _nextReconnectAttempt;

		private Thread _connectionThread;

		// ========= PUBLIC MEMBERS ====================================================================================

		/// <summary>
		/// Setup the client with the specified host and port.
		/// </summary>
		/// <param name="host">Host.</param>
		/// <param name="port">Port.</param>
		public void Setup(string host, int port)
		{
			if (!_IsSetup)
			{
				ClientID = Guid.NewGuid().ToString();
				_brokerEndPoint = new IPEndPoint(IPAddress.Parse(host), port);

				_client = new MQClient(host);
				_client.MqttMsgPublishReceived += OnMessageHandler;
				_client.MqttMsgSubscribed += OnTopicSubscribeHandler;
				_client.MqttMsgUnsubscribed += OnTopicUnsubscribeHandler;

				//_subscribedTopicsMapping = new Dictionary<int, string>(10);
				_subscribedHandlerMapping = new Dictionary<string, Action<string, string>>(10);

				_disposedValue = false;
				_IsSetup = true;

				_connectionThread = new Thread(ConnectionService)
				{
					IsBackground = true,
					Name = "ConnectionService"
				};
				_connectionThread.Start();

				Console.WriteLine("Setup Client {0}", ClientID);
			}
		}

		/// <summary>
		/// Publish data to the specified topic.
		/// </summary>
		/// <param name="topic">Topic.</param>
		/// <param name="data">Data.</param>
		public void Publish(string topic, string data)
		{
			if (_client.IsConnected)
			{
				_client.Publish(topic, Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
			}
		}

		/// <summary>
		/// Subscribe the specified topic.
		/// </summary>
		/// <param name="topic">Topic.</param>
		public void Subscribe(string topic, Action<string, string> handler)
		{
			Subscribe(topic, handler, false);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					try
					{
						_client.Disconnect();
					}
					catch (System.Net.Sockets.SocketException)
					{
						Console.Error.WriteLine("Client not disconnected properly");
					}
					finally
					{
						_IsSetup = false;
						_client = null;
						_brokerEndPoint = null;
					}
				}

				_disposedValue = true;
			}
		}

		// ========= PRIVATE METHODS ===================================================================================

		public void Subscribe(string topic, Action<string, string> handler, bool force)
		{
			if (IsAvailable && (!_subscribedHandlerMapping.ContainsKey(topic) || force))
			{
				if (_client.IsConnected)
				{
					var topicID = _client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

					Console.WriteLine("Client {0} subscribed to topic {1} ({2})", _client.ClientId, topic, topicID);
				}

				if (!_subscribedHandlerMapping.ContainsKey(topic))
				{
					_subscribedHandlerMapping.Add(topic, handler);
				}
			}
		}

		/// <summary>
		/// Handler messages from the subscribed topics.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="evt">Event message</param>
		private void OnMessageHandler(object sender, MqttMsgPublishEventArgs evt)
		{
			if (_subscribedHandlerMapping.TryGetValue(evt.Topic, out Action<string, string> handler))
			{
				handler.Invoke(evt.Topic, Encoding.UTF8.GetString(evt.Message));
			}
		}

		private void OnTopicSubscribeHandler(object sender, MqttMsgSubscribedEventArgs e)
		{
			Console.WriteLine("Topic subscribed {0}", e.MessageId);
		}

		private void OnTopicUnsubscribeHandler(object sender, MqttMsgUnsubscribedEventArgs e)
		{
			Console.WriteLine("Topic unsubscribed {0}", e.MessageId);
		}

		private void ConnectionService()
		{
			Console.WriteLine("Starting Connection Service");

			_nextReconnectAttempt = DateTime.Now;

			while (true)
			{
				if (_IsSetup && !_client.IsConnected)
				{
					if (DateTime.Now.Subtract(_nextReconnectAttempt).TotalMilliseconds > 0)
					{
						try
						{
							Console.WriteLine("Connect procedure");

							_client.Connect(ClientID);

							foreach (var item in _subscribedHandlerMapping)
							{
								Subscribe(item.Key, item.Value, true);
							}
						}
						catch (Exception)
						{
							Console.WriteLine("Error while trying to connect, retry in {0} seconds", CONNECTION_DELAY);
						}
						finally
						{
							_nextReconnectAttempt = DateTime.Now.AddSeconds(CONNECTION_DELAY);
						}
					}
				}

				Thread.Sleep(1);
			}
		}
	}
}

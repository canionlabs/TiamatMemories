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
				return _IsSetup && !_disposedValue && (_subscribedTopics.Count < _subscribedTopics.Capacity);
			}
		}

		public MessageHandler MessageHandler
		{
			set
			{
				if (_messageHandler == null)
				{
					_messageHandler = value;
				}
			}
		}

		public ClientType ClientType => ClientType.MQTT;

		// ========= PRIVATE MEMBERS ===================================================================================

		private const int CONNECTION_DELAY = 5;

		string _clientId;
		MQClient _client;
		IPEndPoint _brokerEndPoint;
		MessageHandler _messageHandler;

		List<string> _subscribedTopics;
		Dictionary<int, string> _subscribedTopicsMapping;

		bool _IsSetup;
		bool _disposedValue;
		DateTime _nextReconnectAttempt;

		Thread _connectionThread;

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
				_clientId = Guid.NewGuid().ToString();
				_brokerEndPoint = new IPEndPoint(IPAddress.Parse(host), port);

				_client = new MQClient(host);
				_client.MqttMsgPublishReceived += OnMessageHandler;
				_client.MqttMsgSubscribed += OnTopicSubscribeHandler;
				_client.MqttMsgUnsubscribed += OnTopicUnsubscribeHandler;

				_subscribedTopics = new List<string>(int.Parse(Settings.MaxTopics));
				_subscribedTopicsMapping = new Dictionary<int, string>(int.Parse(Settings.MaxTopics));

				_disposedValue = false;
				_IsSetup = true;

				_connectionThread = new Thread(ConnectionService)
				{
					IsBackground = true,
					Name = "ConnectionService"
				};
				_connectionThread.Start();

				Console.WriteLine("Setup Client {0}", _clientId);
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
		public void Subscribe(string topic)
		{
			if (IsAvailable && !_subscribedTopics.Contains(topic))
			{
				if (_client.IsConnected)
				{
					var topicID = _client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

					Console.WriteLine("Client {0} subscribed to topic {1}({2})", _client.ClientId, topic, topicID);
				}

				_subscribedTopics.Add(topic);
			}
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

		/// <summary>
		/// Handler messages from the subscribed topics.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="evt">Event message</param>
		private void OnMessageHandler(object sender, MqttMsgPublishEventArgs evt)
		{
			_messageHandler?.Invoke(evt.Topic, Encoding.UTF8.GetString(evt.Message));
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

							_client.Connect(_clientId);

							var savedTopics = _subscribedTopics.ToArray();
							_subscribedTopics.Clear();

							foreach (string topic in savedTopics)
							{
								Subscribe(topic);
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

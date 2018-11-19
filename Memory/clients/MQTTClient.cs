using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Memory.utils;
using uPLibrary.Networking.M2Mqtt.Messages;
using MQClient = uPLibrary.Networking.M2Mqtt.MqttClient;

namespace Memory.clients
{
    public class MQTTClient : IDataResource, IClient
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
                return IsSetup && !disposedValue && (subscribedTopics.Count != subscribedTopics.Capacity);
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

        // ========= PRIVATE MEMBERS ===================================================================================

        bool IsSetup { get; set; }

        private const int CONNECTION_DELAY = 5;

        string clientId;
        IPEndPoint brokerEndPoint;
        MQClient client;

        List<string> subscribedTopics;
        Dictionary<int, string> subscribedTopicsMapping;

        bool disposedValue;
        MessageHandler _messageHandler;

        DateTime lastReconnectAttempt;

        // ========= PRIVATE METHODS ===================================================================================

        /// <summary>
        /// Handler messages from the subscribed topics.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="evt">Event message</param>
        void OnMessageHandler(object sender, MqttMsgPublishEventArgs evt)
        {
            _messageHandler?.Invoke(evt.Topic, Encoding.UTF8.GetString(evt.Message));
        }

        void OnTopicSubscribeHandler(object sender, MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine("Topic subscribed {0}", e.MessageId);
        }

        void OnTopicUnsubscribeHandler(object sender, MqttMsgUnsubscribedEventArgs e)
        {
            Console.WriteLine("Topic unsubscribed {0}", e.MessageId);
        }

        // ========= PUBLIC MEMBERS ====================================================================================

        /// <summary>
        /// Setup the client with the specified host and port.
        /// </summary>
        /// <param name="host">Host.</param>
        /// <param name="port">Port.</param>
        public void Setup(string host, int port)
        {
            if (!IsSetup)
            {
                brokerEndPoint = new IPEndPoint(IPAddress.Parse(host), port);

                client = new MQClient(host);
                client.MqttMsgPublishReceived += OnMessageHandler;
                client.MqttMsgSubscribed += OnTopicSubscribeHandler;
                client.MqttMsgUnsubscribed += OnTopicUnsubscribeHandler;

                subscribedTopics = new List<string>(int.Parse(Settings.MaxTopics));
                subscribedTopicsMapping = new Dictionary<int, string>(int.Parse(Settings.MaxTopics));

                lastReconnectAttempt = DateTime.Now;

                disposedValue = false;
                IsSetup = true;
            }
        }

        /// <summary>
        /// This maintain the instance running
        /// </summary>
        public virtual void Service()
        {
            if (IsSetup && !client.IsConnected)
            {
                // if (DateTime.Now.Subtract(lastReconnectAttempt).TotalSeconds > CONNECTION_DELAY)
                // {
                try
                {
                    // Console.WriteLine("Connect procedure");

                    clientId = Guid.NewGuid().ToString();
                    client.Connect(clientId);

                    var savedTopics = subscribedTopics.ToArray();
                    subscribedTopics.Clear();

                    foreach (string topic in savedTopics)
                    {
                        Subscribe(topic);
                    }
                }
                catch (Exception)
                {
                    // Console.WriteLine("Error while trying to connect, retry in {0} seconds", CONNECTION_DELAY);
                }
                finally
                {
                    // lastReconnectAttempt = DateTime.Now;
                }
                // }
            }
        }

        /// <summary>
        /// Publish data to the specified topic.
        /// </summary>
        /// <param name="topic">Topic.</param>
        /// <param name="data">Data.</param>
        public void Publish(string topic, string data)
        {
            if (client.IsConnected)
            {
                client.Publish(topic, Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }
        }

        /// <summary>
        /// Subscribe the specified topic.
        /// </summary>
        /// <param name="topic">Topic.</param>
        public void Subscribe(string topic)
        {
            if (IsAvailable && !subscribedTopics.Contains(topic))
            {
                if (client.IsConnected)
                {
                    var topicID = client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    
                    Console.WriteLine("Client {0} subscribed to topic {1}({2})", client.ClientId, topic, topicID);
                }

                subscribedTopics.Add(topic);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        client.Disconnect();
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        Console.Error.WriteLine("Client not disconnected properly");
                    }
                    finally
                    {
                        IsSetup = false;
                        client = null;
                        brokerEndPoint = null;
                    }
                }

                disposedValue = true;
            }
        }
    }
}

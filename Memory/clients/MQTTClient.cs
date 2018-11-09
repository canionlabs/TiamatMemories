using System;
using System.Net;
using System.Text;
using Memory.utils;
using uPLibrary.Networking.M2Mqtt.Messages;
using MQClient = uPLibrary.Networking.M2Mqtt.MqttClient;

namespace Memory.clients
{
    public class MQTTClient : IDataResource, IClient
    {
        // ========= PRIVATE MEMBERS ===================================================================================

        bool IsSetup = false;

        string clientId;
        IPEndPoint brokerEndPoint;
        MQClient client;

        private bool disposedValue = false;

        // ========= PRIVATE METHODS ===================================================================================

        void OnMessageHandler(object sender, MqttMsgPublishEventArgs e)
        {
            string msg = Encoding.UTF8.GetString(e.Message);

            Console.WriteLine(e.Topic);
            Console.WriteLine(msg);

            if (msg.Equals("bye"))
            {
                Dispose();
            }
        }

        // ========= PUBLIC MEMBERS ====================================================================================

        public MQTTClient()
        {
            string host = Settings.Read("broker.ip");

            Setup(host);
        }

        public void Setup(string host, int port = 1883)
        {
            if (!IsSetup)
            {
                clientId = Guid.NewGuid().ToString();

                brokerEndPoint = new IPEndPoint(IPAddress.Parse(host), port);

                client = new MQClient(host);
                client.MqttMsgPublishReceived += OnMessageHandler;
                client.Connect(clientId);

                IsSetup = true;
            }
        }

        public void Publish(string topic, string data)
        {
            if (client.IsConnected)
            {
                client.Publish(topic, Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }
        }

        public void Subscribe(string topic)
        {
            if (client.IsConnected)
            {
                client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                Console.WriteLine("Subscribing to {0}", topic);
            }
        }

        public bool Available()
        {
            return IsSetup;
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
                    client.Disconnect();
                    client = null;
                    
                    brokerEndPoint = null;
                    
                    IsSetup = false;
                }
            }
        }
    }
}

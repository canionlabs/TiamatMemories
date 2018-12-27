using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Memory.clients;
using Memory.utils;

namespace Memory
{
	public class MemoryManager : MQTTClient
	{
		public void Start()
		{
			Setup(Settings.Host, int.Parse(Settings.Port));

			MessageHandler = MemoryHandler;

			Subscribe(Settings.MainEntrypoint);
			Console.WriteLine("Memory Ready");
		}

		public void Service()
		{
		}

		public void CleanUp()
		{
			Dispose();
		}

        /// <summary>
        /// Check if a topic is subscribed for more than one resource.
        /// </summary>
        /// <returns><c>true</c>, if subscribe was uniqued, <c>false</c> otherwise.</returns>
        /// <param name="listResources">List resources.</param>
        /// <param name="topic">Topic.</param>
        public bool UniqueSubscribe(List<MQTTClient> listResources, string topic)
        {
            foreach (var resource in listResources)
            {
                if (resource.IsSubscribed(topic))
                {
                    return false;
                }
            }
            return true;
        }

        void MemoryHandler(string topic, string data)
		{
            // Not allowed topics
            if (!topic.StartsWith(Settings.Entrypoint, StringComparison.CurrentCulture))
			{
                return;
			}

			Console.WriteLine("Incoming topic: {0}, data: {1}", topic, data);

			IClient client = ClientFactory<MQTTClient>.BuildClient(Settings.Host, int.Parse(Settings.Port));
            List<MQTTClient> listResources = ClientFactory<MQTTClient>.ListResources;
            if (!client.IsSubscribed(topic) && UniqueSubscribe(listResources, topic))
            {
                client.Subscribe(topic);
                client.MessageHandler = DataHandler;
                client.Publish(topic, data);
            }
        }

		void DataHandler(string topic, string data)
		{
			Console.WriteLine("Saving data {0} from {1}", data, topic);
		}
	}
}

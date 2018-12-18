using System;
using System.Threading.Tasks;
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

			Subscribe(Settings.Entrypoint);
			Console.WriteLine("Memory Ready");
		}

		public override void Service()
		{
			base.Service();
			ClientFactory.Service();
		}

		public void CleanUp()
		{
			ClientFactory.CleanUp();
			Dispose();
		}

		void MemoryHandler(string topic, string data)
		{
			if (topic != Settings.Entrypoint)
			{
				return;
			}

			Console.WriteLine("Incoming topic: {0}", data);

			IClient client = ClientFactory.AcquireClient(ClientType.MQTT);

			client.Subscribe(data);
			client.MessageHandler = DataHandler;
		}

		void DataHandler(string topic, string data)
		{
			Console.WriteLine("Saving data {0} from {1}", data, topic);
		}
	}
}

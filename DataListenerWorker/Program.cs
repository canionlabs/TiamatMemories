using System;
using System.Threading;
using Homie;
using Memory.clients;
using Newtonsoft.Json;

namespace DataListenerWorker
{
	class Program
	{
		static volatile bool running = true;

		static void Main(string[] args)
		{
			//Device device = new Device();
			//object temp = device;

			//Console.WriteLine(((Device)temp).Name);

			ExchangeClient client = new ExchangeClient();

			DeviceManager.Setup(client, "homie");

			client.Setup("127.0.0.1", 1883);

			var device = DeviceManager.CreateDevice("001");
			var device2 = DeviceManager.CreateDevice("002");

			// client.Subscribe("homie/#");

			//client.MessageHandler += (topic, data) =>
			//{
			//Console.WriteLine("Processing {0}", topic);
			//DeviceManager.ProcessStruct(topic, data, "homie", ref temp);
			//};

			int i = 0;
			client.MessageHandler += (topic, data) =>
			{
				Console.WriteLine("\n\n\n[{0}] Summary\n", i++);
				Console.WriteLine(device.ToJson());
			};

			while (running)
			{
				Thread.Sleep(1);
			}

			Console.WriteLine("Bye");
		}

		static void QuitHandler(object sender, ConsoleCancelEventArgs e)
		{
			running = false;
		}
	}
}

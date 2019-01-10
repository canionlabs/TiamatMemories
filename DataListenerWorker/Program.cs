using System;
using System.Text.RegularExpressions;
using System.Threading;
using Amqp;
using Amqp.Framing;
using Homie;
using Homie.Data;
using Memory.clients;

namespace DataListenerWorker
{
	class Program
	{
		static volatile bool running = true;

		static void Main(string[] args)
		{
			Device device = new Device();
			object temp = device;

			Console.WriteLine(((Device)temp).Name);

			MQTTClient client = new MQTTClient();

			client.Setup("127.0.0.1", 1883);
			client.Subscribe("homie/#");

			client.MessageHandler += (topic, data) =>
			{
				Console.WriteLine("Processing {0}", topic);
				DeviceManager.ProcessStruct(topic, data, "homie", ref temp);
			};

			int i = 0;

			client.MessageHandler += (topic, data) =>
			{
				Console.WriteLine("[{0}] Summary", i++);
				Console.WriteLine(device);
			};

			while (running)
			{
				Thread.Sleep(1);
			}

			Console.WriteLine("Bye");

			//var pattern = string.Format(@"^{0}\/\w*\/{1}$", "homie", "$name".Replace("$", "\\$"));
			//var test = "homie/686f6d6965/$name";
			//Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);
			//Console.WriteLine( regex.IsMatch(test) );
		}

		static void QuitHandler(object sender, ConsoleCancelEventArgs e)
		{
			running = false;
		}
	}
}

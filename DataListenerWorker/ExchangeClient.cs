using System;
using Homie.exchange;
using Memory.clients;

namespace DataListenerWorker
{
	// MQTTCLient Adapter
	public class ExchangeClient : MQTTClient, IClientModel
	{
		public void Subscribe(string topic, Action<string, string> handler)
		{
			base.Subscribe(topic);
			this.MessageHandler += handler;
		}
	}
}

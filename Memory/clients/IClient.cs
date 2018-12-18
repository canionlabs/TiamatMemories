using System;
using System.Threading.Tasks;
using Memory.utils;

namespace Memory.clients
{
	public delegate void MessageHandler(string topic, string data);

	public enum ClientType
	{
		MQTT,
		AMQP
	}

	public interface IClient
	{
		MessageHandler MessageHandler { set; }
		ClientType ClientType { get; }

		void Setup(string host, int port);
		void Subscribe(string topic);
		void Publish(string topic, string data);
	}
}

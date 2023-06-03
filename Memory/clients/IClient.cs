using System;
using System.Threading.Tasks;
using Memory.utils;

namespace Memory.clients
{
	public enum ClientType
	{
		MQTT,
		AMQP
	}

	public interface IClient
	{
		//event Action<string, string> MessageHandler;
		ClientType ClientType { get; }

		void Setup(string host, int port);
		void Subscribe(string topic, Action<string, string> handler);
		void Publish(string topic, string data);
	}
}

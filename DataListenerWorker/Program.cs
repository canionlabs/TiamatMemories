using System;
using Amqp;
using Amqp.Framing;

namespace DataListenerWorker
{
	class Program
	{
		static void Main(string[] args)
		{
			Address address = new Address("amqp://guest:guest@localhost:5672");
			Connection connection = new Connection(address);

			Session session = new Session(connection);

			Message message = new Message("Hello AMQP!");
			SenderLink sender = new SenderLink(session, "sender-link", "q1");

			sender.Send(message);
			Console.WriteLine("Sent Hello AMQP!");

			ReceiverLink receiver = new ReceiverLink(session, "receiver-link", "q1");

			sender.Close();
			session.Close();
			connection.Close();
		}
	}
}

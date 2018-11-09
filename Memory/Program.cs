using System;
using Memory.clients;

namespace Memory
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");


            IClient client = ClientFactory.AcquireClient(ClientType.MQTT);

            client.Subscribe("hello");

            //while(!Console.KeyAvailable)
            //{
            //}

            Console.WriteLine("bye");
        }
    }
}

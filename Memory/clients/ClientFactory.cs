using System;
using System.Collections.Generic;
using Memory.utils;

namespace Memory.clients
{
	public enum ClientType
	{
		MQTT
	}

	public static class ClientFactory
	{
		static readonly ResourcePool<MQTTClient> mqttPool;

		static ClientFactory()
		{
			mqttPool = new ResourcePool<MQTTClient>();
		}

		/// <summary>
		/// Returns a usage instance of a certain client type
		/// </summary>
		/// <returns>The client.</returns>
		/// <param name="type">Type of the client. <see cref="Memory.clients.ClientType"/>.</param>
		public static IClient AcquireClient(ClientType type)
		{
			IClient client = null;

			switch (type)
			{
				case ClientType.MQTT:
					client = mqttPool.AcquireResource();
					break;
			}

			if (client != null)
			{
				client.Setup(Settings.Host, int.Parse(Settings.Port));
			}

			return client;
		}

		public static void Service()
		{
			mqttPool.Service();
		}

		public static void CleanUp()
		{
			mqttPool.CleanUp();
		}
	}
}

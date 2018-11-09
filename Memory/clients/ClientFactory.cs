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

        public static IClient AcquireClient(ClientType type)
        {
            switch(type)
            {
                case ClientType.MQTT:
                    return mqttPool.AcquireResource();
            }

            return null;
        }
    }
}

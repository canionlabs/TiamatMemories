using System;
using Memory.utils;

namespace Memory.clients
{
    public interface IClient
    {
        void Setup(string host, int port);
        void Subscribe(string topic);
        void Publish(string topic, string data);
    }
}

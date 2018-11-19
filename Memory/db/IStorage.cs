using System;
namespace Memory.db
{
    public interface IStorage
    {
        void Setup();
        void Save(string @namespace, string data);
    }
}

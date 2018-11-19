using System;
namespace Memory.utils
{
    public interface IDataResource : IDisposable
    {
        void Service();
        bool IsAvailable { get; }
    }
}

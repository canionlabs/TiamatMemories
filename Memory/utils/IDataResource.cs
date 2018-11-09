using System;
namespace Memory.utils
{
    public interface IDataResource : IDisposable
    {
        bool Available();
    }
}

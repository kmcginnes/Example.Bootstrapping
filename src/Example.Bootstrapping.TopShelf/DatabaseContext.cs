using System;

namespace Example.Bootstrapping.TopShelf
{
    public class DatabaseContext : IDisposable
    {
        public DatabaseContext()
        {
            this.Log().Debug($"Inside ctor()");
        }

        public void Dispose()
        {
            this.Log().Debug("Inside Dispose()");
        }
    }
}
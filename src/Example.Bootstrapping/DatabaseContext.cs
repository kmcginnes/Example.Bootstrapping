using System;

namespace Example.Bootstrapping
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
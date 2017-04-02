using System;
using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public class DatabaseContext : IDisposable
    {
        public DatabaseContext()
        {
            this.Log().Debug($"{nameof(DatabaseContext)}.ctor()");
        }

        public void Dispose()
        {
            this.Log().Debug($"{nameof(DatabaseContext)}.Dispose()");
        }

        public Task QueryAsync()
        {
            this.Log().Debug($"Querying the database for stuff...");
            return Task.Delay(TimeSpan.FromMilliseconds(200));
        }
    }
}
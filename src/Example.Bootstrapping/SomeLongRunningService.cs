using System;
using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public class SomeLongRunningService : ILongRunningService
    {
        public Task Initialize()
        {
            this.Log().Info($"Initializing {nameof(SomeLongRunningService)}");
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();
            this.Log().Info($"Finished initializing {nameof(SomeLongRunningService)}");
            return Task.FromResult(true);
        }
    }
}
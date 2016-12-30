using System;
using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public class MisbehavingService : ILongRunningService
    {
        public Task Initialize()
        {
            this.Log().Info($"Initializing {nameof(MisbehavingService)}");
            throw new NotImplementedException();
            this.Log().Info($"Finished initializing {nameof(MisbehavingService)}");
        }
    }
}
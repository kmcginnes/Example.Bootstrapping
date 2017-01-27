using System;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public class SomeLongRunningService : ILongRunningService, IDisposable
    {
        private readonly CancellationTokenSource _tokenSource;
        private readonly Thread _thread;

        public SomeLongRunningService()
        {
            _tokenSource = new CancellationTokenSource();
            _thread = new Thread(Execute);
        }

        public Task Initialize()
        {
            this.Log().Info($"Initializing {nameof(SomeLongRunningService)}");
            _thread.Start(_tokenSource.Token);
            this.Log().Info($"Finished initializing {nameof(SomeLongRunningService)}");
            return Task.FromResult(true);
        }

        public void Execute(object param)
        {
            var token = (CancellationToken) param;
            while (!token.IsCancellationRequested)
            {
                this.Log().Debug("Still working...");
                Thread.Sleep(1000);
            }
            this.Log().Debug("Cancelled our work");
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
            this.Log().Debug("Inside Dispose()");
        }
    }
}
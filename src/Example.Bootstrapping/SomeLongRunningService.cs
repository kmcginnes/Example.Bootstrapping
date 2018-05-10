using System;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public class SomeLongRunningService : ILongRunningService, IDisposable
    {
        private readonly CancellationTokenSource _tokenSource;
        private readonly RepeatingTask _repeatingTask;

        public SomeLongRunningService()
        {
            _tokenSource = new CancellationTokenSource();
            _repeatingTask = new RepeatingTask(TimeSpan.FromSeconds(5), ExecuteAsync);
        }

        public Task Initialize()
        {
            this.Log().Info($"Initializing {nameof(SomeLongRunningService)}");
            _repeatingTask.Start();
            this.Log().Info($"Finished initializing {nameof(SomeLongRunningService)}");
            return Task.FromResult(true);
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            this.Log().Info($"Working on something...");
            await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
            this.Log().Info($"Still Working on something...");
            await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
            this.Log().Info($"Still Working on something...");
            await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
            this.Log().Info($"Still Working on something...");
            await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);

            this.Log().Info($"Done working on something.");
        }

        public void Dispose()
        {
            this.Log().Debug("Inside Dispose()");
            _tokenSource.Cancel();
            this.Log().Debug("Stopping the repeating task...");
            _repeatingTask.Stop();
        }
    }
}
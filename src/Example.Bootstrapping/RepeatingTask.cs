using System;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public class RepeatingTask
    {
        private readonly TimeSpan _interval;
        private readonly Func<CancellationToken, Task> _task;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Timer _timer;

        public RepeatingTask(TimeSpan interval, Func<CancellationToken, Task> task)
        {
            _interval = interval;
            _task = task;
            _cancellationTokenSource = new CancellationTokenSource();
            _timer = new Timer(OnTimerCallbackAsync, null, Timeout.Infinite, Timeout.Infinite);
        }

        private async void OnTimerCallbackAsync(object state)
        {
            try
            {
                StopTimer();
                this.Log().Debug("Calling repeating task...");
                await _task(_cancellationTokenSource.Token);
                ResumeTimer();
            }
            catch (TaskCanceledException)
            {
                // Don't care about this guy
            }
            catch (Exception exception)
            {
                this.Log().Error(exception, $"Uncaught exception executing repeating task.");
            }
        }

        private void StopTimer()
        {
            this.Log().Debug("Setting timer to paused...");
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void ResumeTimer()
        {
            this.Log().Debug($"Resuming timer with interval {_interval}...");
            _timer.Change((int)_interval.TotalMilliseconds, (int)_interval.TotalMilliseconds);
        }

        public void Start()
        {
            this.Log().Debug($"Starting timer with interval {_interval}...");
            _timer.Change(0, (int)_interval.TotalMilliseconds);
        }

        public void Stop()
        {
            StopTimer();
            this.Log().Debug("Cancelling the cancellation token source...");
            _cancellationTokenSource.Cancel();
        }
    }
}

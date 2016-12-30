using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public class LongRunningServiceOrchestrator
    {
        private readonly IEnumerable<Func<ILongRunningService>> _services;

        public LongRunningServiceOrchestrator(IEnumerable<Func<ILongRunningService>> services)
        {
            _services = services;
        }

        public async Task StartLongRunningServices()
        {
            var tasks = _services.Select(factory =>
                Task.Run(async () => await InitializeServiceWithFactory(factory)));

            await Task.WhenAll(tasks);
        }

        private async Task InitializeServiceWithFactory<T>(Func<T> factory) where T : ILongRunningService
        {
            // Constructors should never throw exceptions, but IoC binding sometimes does.
            T instance;
            try
            {
                instance = factory();
            }
            catch (Exception exception)
            {
                this.Log().Error(exception, $"Failed to instantiate long running service.");
#if DEBUG
                Debugger.Break();
#endif
                return;
            }

            var nameOfService = instance.GetType().Name;
            try
            {
                this.Log().Debug($"Initializing long running service {nameOfService}.");

                await instance.Initialize().ConfigureAwait(false);
                this.Log().Debug($"Long running service {nameOfService} is now running.");
            }
            catch (Exception exception)
            {
                this.Log().Error(exception, $"Failed to initialize {nameOfService}.");
#if DEBUG
                //Debugger.Break();
#endif
            }
        }
    }
}

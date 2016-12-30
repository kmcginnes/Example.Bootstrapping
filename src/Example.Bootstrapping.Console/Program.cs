using System;

namespace Example.Bootstrapping.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Bootstrapper();

            app.Start(args).Wait(TimeSpan.FromMinutes(2));
            
            // These would be from an IoC container
            var services = new Func<ILongRunningService>[]
            {
                () => new MisbehavingService(),
                () => new SomeLongRunningService(),
            };

            var orchestrator = new LongRunningServiceOrchestrator(services);
            orchestrator.StartLongRunningServices().Wait();

            typeof(Program).Name.Log().Info("Listening for key press...");
            System.Console.ReadKey();
        }
    }
}

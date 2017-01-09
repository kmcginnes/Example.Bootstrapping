using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Example.Bootstrapping.TopShelf
{
    public class Bootstrapper
    {
        public void Start(string[] commandLineArgs)
        {
            var banner = new StringBuilder();
            banner.AppendLine(@" ______               __          __                                     ");
            banner.AppendLine(@"|   __ \.-----.-----.|  |_.-----.|  |_.----.---.-.-----.-----.-----.----.");
            banner.AppendLine(@"|   __ <|  _  |  _  ||   _|__ --||   _|   _|  _  |  _  |  _  |  -__|   _|");
            banner.AppendLine(@"|______/|_____|_____||____|_____||____|__| |___._|   __|   __|_____|__|  ");
            banner.AppendLine(@"                                                 |__|  |__|              ");
            banner.AppendLine(@"    ");
            var logging = new LoggingOrchestrator();
            logging.InitializeLogging("Main", banner.ToString());
            
            GlobalExceptionHandlers.WireUp();

            const string appId = "bootstrapping-topshelf";
            var environment = new EnvironmentFacade(Assembly.GetExecutingAssembly());

            var appSettings = ConfigurationParser.Parse(commandLineArgs, ConfigurationManager.AppSettings);
            logging.LogUsefulInformation(environment, appSettings, appId);

            this.Log().Debug("Initializing the IoC container...");
            var container = InitializeContainer(appSettings);

            this.Log().Debug($"Finished bootstrapping {appId}");

            // These would be from an IoC container
            var services = new Func<ILongRunningService>[]
            {
                () => new MisbehavingService(),
                () => new SomeLongRunningService(),
            };

            var orchestrator = new LongRunningServiceOrchestrator(services);
            orchestrator.StartLongRunningServices().Wait();

            var commandProcessors = new IConsoleCommandProcessor[]
            {
                new QuitConsoleCommandProcessor(), new SweepKickoffConsoleCommandProcessor(),
            };

            if (Environment.UserInteractive)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(200));
                    while (true)
                    {
                        await ParseMenuCommands(commandProcessors);
                    }
                });
            }
        }

        private async Task ParseMenuCommands(IConsoleCommandProcessor[] commandProcessors)
        {
            this.Log().Info("");
            this.Log().Info($"Possible console commands:");
            foreach (var commandProcessor in commandProcessors)
            {
                this.Log().Info($"        '{commandProcessor.InputCharacter}' = {commandProcessor.LongName}");
            }
            this.Log().Info("");
            var input = System.Console.ReadKey(true);
            var processor = commandProcessors
                .FirstOrDefault(x => x.InputCharacter == input.KeyChar);
            if (processor != null)
            {
                await processor.Execute();
            }
            else
            {
                this.Log().Warn($"Did not find a processor for command '{input.KeyChar}'.");
            }
        }

        private IDependencyContainer InitializeContainer(AppSettings appSettings)
        {
            return (IDependencyContainer)null;
        }

        public void Stop()
        {
            
        }
    }
}
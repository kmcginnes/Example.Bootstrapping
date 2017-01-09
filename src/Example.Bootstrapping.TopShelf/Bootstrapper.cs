using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.Windsor.Proxy;

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

            this.Log().Debug($"Finished bootstrapping {appId}.");

            // Kick off long running services
            var orchestrator = container.Resolve<LongRunningServiceOrchestrator>();
            orchestrator.StartLongRunningServices().Wait();

            this.Log().Info($"All long running services are started.");

            if (Environment.UserInteractive)
            {
                Task.Run(async () =>
                {
                    this.Log().Info($"Interactive console mode detected.");
                    var commandProcessors = container.ResolveAll<IConsoleCommandProcessor>();

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

        private IWindsorContainer InitializeContainer(IAppSettings appSettings)
        {
            // Basic Castle Windsor setup that all apps should use.
            var container = new WindsorContainer(
                new DefaultKernel(
                    new ArgumentPassingDependencyResolver(), // overrides for inline argument passing
                    new DefaultProxyFactory()),
                new DefaultComponentInstaller());

            // Turn off automatic property injection
            container.Kernel.ComponentModelBuilder.RemoveContributor(
                container.Kernel.ComponentModelBuilder
                    .Contributors.OfType<PropertiesDependenciesModelInspector>().Single()
            );

            // Allow Lazy<T> of any services
            container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LazyOfTComponentLoader>());

            // Bootstrapper registrations
            container.Register(Component.For<IAppSettings>().Instance(appSettings));

            // Register all custom installers
            container.Install(FromAssembly.InThisApplication());

            return container;
        }

        public void Stop()
        {
            
        }
    }
}
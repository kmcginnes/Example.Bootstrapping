using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.Windsor.Proxy;
using Example.Bootstrapping.TopShelf.CastleWindsor;

namespace Example.Bootstrapping.TopShelf
{
    public class Bootstrapper
    {
        private readonly List<IDisposable> _disposableBag = new List<IDisposable>();

        public void Start(string[] commandLineArgs)
        {
            Log4NetConfig.Setup();
            var banner = new StringBuilder();
            banner.AppendLine(@" ______               __          __                                     ");
            banner.AppendLine(@"|   __ \.-----.-----.|  |_.-----.|  |_.----.---.-.-----.-----.-----.----.");
            banner.AppendLine(@"|   __ <|  _  |  _  ||   _|__ --||   _|   _|  _  |  _  |  _  |  -__|   _|");
            banner.AppendLine(@"|______/|_____|_____||____|_____||____|__| |___._|   __|   __|_____|__|  ");
            banner.AppendLine(@"                                                 |__|  |__|              ");
            banner.AppendLine(@"    ");
            var logging = new LoggingOrchestrator();
            logging.InitializeLogging<Log4NetLog>("Main", banner.ToString());
            
            GlobalExceptionHandlers.WireUp();

            System.Console.OutputEncoding = Encoding.UTF8;

            var environment = new EnvironmentFacade(Assembly.GetExecutingAssembly());

            var appSettings = ConfigurationParser.Parse(commandLineArgs, ConfigurationManager.AppSettings);
            logging.LogUsefulInformation(environment, appSettings);

            var container = InitializeContainer(appSettings);
            _disposableBag.Add(container);

            this.Log().Debug($"Finished bootstrapping {environment.GetProductName()}.");

            // Kick off long running services
            var orchestrator = container.Resolve<LongRunningServiceOrchestrator>();
            orchestrator.StartLongRunningServices()
                .ContinueWith(_ =>
                {
                    this.Log().Info($"All long running services are started.");

                    // This code will listen to console key presses and execute the code 
                    // associated with it
                    if (Environment.UserInteractive)
                    {
                        this.Log().Debug($"Interactive console mode detected.");
                        var commandProcessor = container.Resolve<ConsoleCommandOrchestrator>();
                        commandProcessor.StartUp();
                    }
                });
        }

        private IWindsorContainer InitializeContainer(IAppSettings appSettings)
        {
            this.Log().Debug("Initializing the IoC container with Castle Windsor...");

            var container = CreateWindsorContainer();

            // Bootstrapper registrations
            container.Register(Component.For<IAppSettings>().Instance(appSettings));

            // Register all custom installers
            container.Install(FromAssembly.InThisApplication());

            return container;
        }

        private static WindsorContainer CreateWindsorContainer()
        {
            // Basic Castle Windsor setup that all apps should use.

            // Allows an inline parameter, usually from a factory, to be passed
            // down multiple layers of constructors
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

            // Allow IEnumerable<T> of any services
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

            // Allow contravariance in generic resolution
            container.Kernel.AddHandlersFilter(new ContravariantFilter());

            return container;
        }

        public void Stop()
        {
            foreach (var disposable in _disposableBag)
            {
                disposable.Log().Debug($"Disposing...");
                disposable.Dispose();
            }
        }
    }
}
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Example.Bootstrapping.Logging;
using MediatR;

namespace Example.Bootstrapping.Console
{
    public class Bootstrapper
    {
        public async Task Start(string[] commandLineArgs)
        {
            var banner = new StringBuilder();
            banner.AppendLine(@" ______               __          __                                     ");
            banner.AppendLine(@"|   __ \.-----.-----.|  |_.-----.|  |_.----.---.-.-----.-----.-----.----.");
            banner.AppendLine(@"|   __ <|  _  |  _  ||   _|__ --||   _|   _|  _  |  _  |  _  |  -__|   _|");
            banner.AppendLine(@"|______/|_____|_____||____|_____||____|__| |___._|   __|   __|_____|__|  ");
            banner.AppendLine(@"                                                 |__|  |__|              ");
            banner.AppendLine(@"    ");
            var logging = new LoggingOrchestrator();
            logging.InitializeLogging<ConsoleAndFileLogger>("Main", banner.ToString());

            GlobalExceptionHandlers.WireUp();

            System.Console.OutputEncoding = Encoding.UTF8;

            var environment = new EnvironmentFacade(Assembly.GetExecutingAssembly());

            var appSettings = ConfigurationParser.Parse(commandLineArgs, ConfigurationManager.AppSettings);
            logging.LogUsefulInformation(environment, appSettings);

            var container = InitializeContainer(appSettings);

            (this).Log().Debug($"Finished bootstrapping {environment.GetProductName()}");

            // Kick off long running services
            var orchestrator = container.Resolve<LongRunningServiceOrchestrator>();
            await orchestrator.StartLongRunningServices();
            this.Log().Info($"All long running services are started.");

            // This code will listen to console key presses and execute the code 
            // associated with it
            this.Log().Debug($"Interactive console mode detected.");
            var commandProcessor = container.Resolve<ConsoleCommandOrchestrator>();
            commandProcessor.StartUp();
        }
        
        private IContainer InitializeContainer(AppSettings appSettings)
        {
            this.Log().Debug("Initializing the IoC container with AutoFac...");

            var assemblies = new [] { typeof(Bootstrapper).Assembly, typeof(EnvironmentFacade).Assembly };
            var builder = new ContainerBuilder();
            // Don't use ContravariantRegistrationSource in AutoFac or it will create many closed generic types
            // for any of your open generic registrations (like LoggingBehavior<,> below).
            //builder.RegisterSource(new ContravariantRegistrationSource());
            
            builder.RegisterInstance(appSettings).AsImplementedInterfaces();

            builder.RegisterType<LongRunningServiceOrchestrator>().SingleInstance();
            builder.RegisterType<ConsoleCommandOrchestrator>().SingleInstance();
            builder.RegisterType<DatabaseContext>();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.IsAssignableTo<ILongRunningService>() || t.IsAssignableTo<IConsoleCommandProcessor>())
                .AsImplementedInterfaces()
                .SingleInstance();
            
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();
            
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IRequestHandler<>)).InstancePerDependency();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IRequestHandler<,>)).InstancePerDependency();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IPipelineBehavior<,>)).InstancePerDependency();
            
            // Must register because AutoFac does not know how to scan for these
            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>)).InstancePerDependency();

            var container = builder.Build();
            return container;
        }
    }
}

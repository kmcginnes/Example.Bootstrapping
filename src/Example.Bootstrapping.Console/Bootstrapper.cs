using System;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
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
            logging.InitializeLogging("Main", banner.ToString());

            GlobalExceptionHandlers.WireUp();

            const string appId = "bootstrapping-console";
            var environment = new EnvironmentFacade(Assembly.GetExecutingAssembly());

            var appSettings = ConfigurationParser.Parse(commandLineArgs, ConfigurationManager.AppSettings);
            logging.LogUsefulInformation(environment, appSettings, appId);

            var container = InitializeContainer(appSettings);

            this.Log().Debug($"Finished bootstrapping {appId}");

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
            var hackToRegisterContainer = new Lazy<IContainer>(() => builder.Build());

            builder.RegisterInstance(appSettings).AsImplementedInterfaces();

            builder.RegisterType<LongRunningServiceOrchestrator>().SingleInstance();
            builder.RegisterType<ConsoleCommandOrchestrator>().SingleInstance();
            builder.RegisterType<DatabaseContext>();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.IsAssignableTo<ILongRunningService>() || t.IsAssignableTo<IConsoleCommandProcessor>())
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register(c => hackToRegisterContainer.Value);

            builder.Register(ctx => (Func<ILifetimeScope>)(() => hackToRegisterContainer.Value.BeginLifetimeScope()));
            builder.RegisterType<ScopedMediator>().As<IMediator>();

            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IAsyncRequestHandler<>)).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IAsyncRequestHandler<,>)).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IRequestHandler<>)).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IRequestHandler<,>)).InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IPipelineBehavior<,>)).InstancePerLifetimeScope();

            var container = hackToRegisterContainer.Value;
            return container;
        }
    }
}
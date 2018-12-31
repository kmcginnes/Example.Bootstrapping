using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Example.Bootstrapping.TopShelf.CastleWindsor;
using Example.Bootstrapping.TopShelf.FindJobs;
using MediatR;

namespace Example.Bootstrapping.TopShelf
{
    public class AppInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Register console commands
            container.Register(Component.For<ConsoleCommandOrchestrator>().LifestyleSingleton());
            container.Register(Component.For<IConsoleCommandProcessor>().ImplementedBy<QuitTopShelfServiceCommandProcessor>());
            container.Register(Component.For<IConsoleCommandProcessor>().ImplementedBy<FindJobsConsoleCommandProcessor>());
            container.Register(Component.For<IConsoleCommandProcessor>().ImplementedBy<SweepKickoffConsoleCommandProcessor>());

            // Register long running services
            container.Register(Component.For<LongRunningServiceOrchestrator>().LifestyleSingleton());
            container.Register(
                Classes.FromAssemblyInThisApplication()
                    .BasedOn(typeof(ILongRunningService))
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton());

            container.Register(Component.For<IEnumerable<Func<ILongRunningService>>>()
                .UsingFactoryMethod(ListOfFactoryMethods<ILongRunningService>));

            // Register MediatR related types
            container.Register(Component.For<IMediator>().ImplementedBy<ScopedMediator>());
            
            container.Register(
                Classes.FromAssemblyInThisApplication()
                    .BasedOn(typeof(IRequestHandler<>))
                    .OrBasedOn(typeof(IRequestHandler<,>))
                    .OrBasedOn(typeof(IPipelineBehavior<,>))
                    .WithServiceAllInterfaces()
                    .LifestyleScoped());

            container.Register(Component.For<DatabaseContext>().LifestyleTransient());
        }

        private IEnumerable<Func<T>> ListOfFactoryMethods<T>(IKernel kernel, CreationContext context)
        {
            var handlers = kernel.GetHandlers(typeof(T));

            return handlers.Select(handler =>
            {
                Func<T> factory = () => (T) handler.Resolve(context);
                return factory;
            });
        }
    }
}

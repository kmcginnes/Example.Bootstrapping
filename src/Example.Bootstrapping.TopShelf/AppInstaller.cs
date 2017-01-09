using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Example.Bootstrapping.TopShelf
{
    public class AppInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<LongRunningServiceOrchestrator>().LifestyleSingleton());
            container.Register(
                Classes.FromAssemblyInThisApplication()
                    .BasedOn(typeof(ILongRunningService), typeof(IConsoleCommandProcessor))
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton());

            container.Register(Component.For<IEnumerable<Func<ILongRunningService>>>()
                .UsingFactoryMethod((kernel, context) =>
                {
                    var handlers = kernel.GetHandlers(typeof(ILongRunningService));

                    return handlers.Select(handler =>
                    {
                        Func<ILongRunningService> factory = () => (ILongRunningService) handler.Resolve(context);
                        return factory;
                    });
                }));
        }
    }
}

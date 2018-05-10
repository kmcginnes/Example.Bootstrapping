﻿using Topshelf;

namespace Example.Bootstrapping.TopShelf
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(cfg =>
            {
                cfg.Service<Bootstrapper>(s =>
                {
                    s.ConstructUsing(() => new Bootstrapper());
                    s.WhenStarted((bootstrapper, hostControl) => bootstrapper.Start(args, hostControl));
                    s.WhenStopped(x => x.Stop());
                });
                cfg.OnException(ex => GlobalExceptionHandlers.OnException(ex, "Windows service"));
                cfg.SetServiceName("bootstrapping-topshelf");
                cfg.SetDescription("Example Bootstrapping TopShelf App");
            });
        }
    }
}

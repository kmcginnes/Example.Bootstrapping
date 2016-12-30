﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Example.Bootstrapping
{
    public static class GlobalExceptionHandlers
    {
        private static readonly ILog Logger = typeof(GlobalExceptionHandlers).Name.Log();

        public static void WireUp()
        {
            Logger.Debug("Wiring up global exception handlers...");
            
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var logger = sender?.Log() ?? Logger;
                var exception = args.ExceptionObject as Exception;
                logger.Fatal(exception, "Unhandled exception in the app domain. Exiting with exit code 1.");
#if DEBUG
                Debugger.Break();
#else
                Environment.Exit(1);
#endif
            };
            Logger.Debug("Wired up AppDomain.CurrentDomain.UnhandledException.");


            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                var logger = sender?.Log() ?? Logger;
                logger.Fatal(args.Exception, "Unhandled exception in the task scheduler.");
#if DEBUG
                Debugger.Break();
#else
                Environment.Exit(1);
#endif
            };
            Logger.Debug("Wired up TaskScheduler.UnobservedTaskException.");
        }
    }
}
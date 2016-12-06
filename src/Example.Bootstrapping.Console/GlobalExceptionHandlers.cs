using System;
using System.Threading.Tasks;

namespace Example.Bootstrapping.Console
{
    public static class GlobalExceptionHandlers
    {
        public static void WireUp()
        {
            // If we are running this app in Visual Studio then do not handle
            // any of the unhandled exceptions. Let Visual Studio catch them.
            if (AppDomain.CurrentDomain.FriendlyName.EndsWith("vshost.exe"))
            {
                return;
            }

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var loggerTarget = sender ?? typeof(GlobalExceptionHandlers).Name;
                var exception = args.ExceptionObject as Exception;
                loggerTarget.Log().Fatal(exception, "Unhandled exception in the app domain.");
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                var loggerTarget = sender ?? typeof(GlobalExceptionHandlers).Name;
                loggerTarget.Log().Fatal(args.Exception, "Unhandled exception in the task scheduler.");
            };
        }
    }
}
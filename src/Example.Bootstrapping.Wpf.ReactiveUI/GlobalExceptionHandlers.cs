using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Example.Bootstrapping.Logging;

namespace Example.Bootstrapping.Wpf.ReactiveUI
{
    public static class GlobalExceptionHandlers
    {
        private static readonly ILog Logger = nameof(GlobalExceptionHandlers).Log();

        public static void WireUp()
        {
            Logger.Debug("Wiring up global exception handlers.");
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var logger = sender?.Log() ?? Logger;
                var exception = args.ExceptionObject as Exception;
                logger.Fatal(exception, "Unhandled exception in the app domain. Exiting with exit code 1.");
#if DEBUG
                Debugger.Break();
#else
                ShowMessageAndExit();
#endif
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                var logger = sender?.Log() ?? Logger;
                logger.Fatal(args.Exception, "Unhandled exception in the task scheduler. Exiting with exit code 1.");
#if DEBUG
                Debugger.Break();
#else
                ShowMessageAndExit();
#endif
            };

            Application.Current.DispatcherUnhandledException += (sender, args) =>
            {
                var loggerTarget = sender ?? Logger;
                loggerTarget.Log().Fatal(args.Exception, "Unhandled exception in the application dispatcher.");
#if DEBUG
                Debugger.Break();
#else
                ShowMessageAndExit();
#endif
            };
        }

        private static void ShowMessageAndExit()
        {
            var owner = Application.Current.MainWindow;
            var message = "An unhandled exception occurred. Check the logs for more details.";
            var title = "Fatal Application Error";
            MessageBox.Show(owner, message, title, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            Environment.Exit(1);
        }

        public static void OnException(Exception exception, string exceptionSource)
        {
            Logger.Fatal(exception, $"Unhandled exception in the {exceptionSource}. Exiting with exit code 1.");
#if DEBUG
            Debugger.Break();
#else
            ShowMessageAndExit();
#endif
        }
    }
}

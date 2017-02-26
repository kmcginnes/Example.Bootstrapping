using System;
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Example.Bootstrapping.Logging;

namespace Example.Bootstrapping.Wpf.ReactiveUI
{
    public class Bootstrapper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [STAThread]
        public static void Main(string[] args)
        {
            FreeConsole();

            new Bootstrapper().Run(args);
        }

        private void Run(string[] args)
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

            var app = new App { ShutdownMode = ShutdownMode.OnLastWindowClose };

            GlobalExceptionHandlers.WireUp();

            var environment = new EnvironmentFacade(Assembly.GetExecutingAssembly());

            var appSettings = ConfigurationParser.Parse(args, ConfigurationManager.AppSettings);
            logging.LogUsefulInformation(environment, appSettings);

            app.Exit += (s, e) => app.Log().Info($"{environment.GetProductName()} is exiting");
            app.InitializeComponent();

            var shell = new ShellView {ViewModel = new ShellViewModel()};
            shell.Show();

            app.Run();
            this.Log().Debug($"{environment.GetProductName()} has exited.");
        }
    }
}

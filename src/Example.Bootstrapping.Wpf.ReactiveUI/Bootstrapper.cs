﻿using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Serilog;

namespace Example.Bootstrapping.Wpf.ReactiveUI
{
    public class Bootstrapper
    {
        private EnvironmentFacade _environment;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [STAThread]
        public static int Main(string[] args)
        {
            FreeConsole();

            return new Bootstrapper().Run(args);
        }

        private int Run(string[] args)
        {
            Serilog.Log.Logger = CreateCoreLogger();

            var banner = new StringBuilder();
            banner.AppendLine(@" ______               __          __                                     ");
            banner.AppendLine(@"|   __ \.-----.-----.|  |_.-----.|  |_.----.---.-.-----.-----.-----.----.");
            banner.AppendLine(@"|   __ <|  _  |  _  ||   _|__ --||   _|   _|  _  |  _  |  _  |  -__|   _|");
            banner.AppendLine(@"|______/|_____|_____||____|_____||____|__| |___._|   __|   __|_____|__|  ");
            banner.AppendLine(@"                                                 |__|  |__|              ");
            banner.AppendLine(@"    ");
            var logging = new LoggingOrchestrator();
            logging.InitializeLogging<SerilogAdapter>("Main", banner.ToString());

            var app = new App { ShutdownMode = ShutdownMode.OnLastWindowClose };

            GlobalExceptionHandlers.WireUp();

            _environment = new EnvironmentFacade(Assembly.GetExecutingAssembly());

            var appSettings = ConfigurationParser.Parse(args, ConfigurationManager.AppSettings);
            logging.LogUsefulInformation(_environment, appSettings);

            app.Exit += OnExit;
            app.InitializeComponent();

            var shell = new ShellView {ViewModel = new ShellViewModel(new ShellViewModelValidator())};
            shell.Show();

            var exitCode = app.Run();
            this.Log().Debug($"{_environment.GetProductName()} has exited with code {exitCode}.");
            Serilog.Log.CloseAndFlush();
            return exitCode;
        }

        private void OnExit(object s, ExitEventArgs e)
        {
            s.Log().Info($"{_environment.GetProductName()} is exiting");
        }

        private ILogger CreateCoreLogger()
        {
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var logPath = Path.Combine(programData, "Example.Bootstapping.Wpf.ReactiveUI", "app.log");

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: logPath,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3} {ThreadId} {Properties}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            return logger;
        }
    }
}

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Bootstrapping.Console
{
    public class Bootstrapper
    {
        public Task Start(string[] commandLineArgs)
        {
            InitializeLogging();
            GlobalExceptionHandlers.WireUp();

            var appContext = CreateAppContext();
            LogUsefulInformation(appContext);

            var appSettings = ConfigurationParser.Parse(commandLineArgs, ConfigurationManager.AppSettings);

            ConfigurationParser.LogSettings(appSettings);

            var container = InitializeContainer(appSettings);
            
            return Task.FromResult(true);
        }
        
        private IContainer InitializeContainer(AppSettings appSettings)
        {
            return (IContainer) null;
        }

        private void LogUsefulInformation(IAppContext appContext)
        {
            this.Log().Info($"Starting {appContext.AppId} v{appContext.AssemblyVersion}");

            var ipAddress =
                Dns.GetHostEntry(Dns.GetHostName())
                    .AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            var windowsVersion = $"{Environment.OSVersion} {(Environment.Is64BitOperatingSystem ? "64bit" : "32bit")}";

            this.Log().Info($"Assembly location: {appContext.AssemblyLocation}");
            this.Log().Info($" Assembly version: {appContext.AssemblyVersion}");
            this.Log().Info($"     File version: {appContext.AssemblyFileVersion}");
            this.Log().Info($"       Running as: {appContext.PrincipalName}");
            this.Log().Info($"     Network Host: {appContext.HostName} ({ipAddress})");
            this.Log().Info($"  Windows Version: {windowsVersion}");
        }

        private void InitializeLogging()
        {
            // Set the main thread's name to make it clear in the logs.
            if (Thread.CurrentThread.Name != "Main")
                Thread.CurrentThread.Name = "Main";

            // Sets my logger to the console, which goes to the debug output.
            Log.InitializeWith<ConsoleLog>();

            // Show a banner to easily pick out where new instances start
            // in the log file. Plus it just looks cool.
            this.Log().Info(@" ______               __          __                                     ");
            this.Log().Info(@"|   __ \.-----.-----.|  |_.-----.|  |_.----.---.-.-----.-----.-----.----.");
            this.Log().Info(@"|   __ <|  _  |  _  ||   _|__ --||   _|   _|  _  |  _  |  _  |  -__|   _|");
            this.Log().Info(@"|______/|_____|_____||____|_____||____|__| |___._|   __|   __|_____|__|  ");
            this.Log().Info(@"                                                 |__|  |__|              ");
            this.Log().Info(@"    ");
        }

        private static IAppContext CreateAppContext()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var assemblyLocation = Path.GetDirectoryName(path);

            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var fileVersion = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
            var principal = WindowsIdentity.GetCurrent().Name ?? "[Unknown]";
            var hostName = Environment.MachineName;

            return new ExampleBootstrappingAppContext(hostName, String.Empty, principal, assemblyLocation, assemblyVersion, fileVersion);
        }
    }
}
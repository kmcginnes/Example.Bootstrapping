using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Example.Bootstrapping
{
    public class LoggingOrchestrator
    {
        private readonly string _banner;
        private readonly string _mainThreadName;

        public LoggingOrchestrator(string mainThreadName, string banner)
        {
            _banner = banner;
            _mainThreadName = mainThreadName;
        }

        public void InitializeLogging()
        {
            // Set the main thread's name to make it clear in the logs.
            if (Thread.CurrentThread.Name != _mainThreadName)
                Thread.CurrentThread.Name = _mainThreadName;

            Console.OutputEncoding = Encoding.UTF8;

            // Sets my logger to the console, which goes to the debug output.
            Log.InitializeWith<ConsoleLog>();

            // Show a banner to easily pick out where new instances start
            // in the log file. Plus it just looks cool.
            _banner.Split(new[] {Environment.NewLine}, StringSplitOptions.None)
                .Do(x => this.Log().Info(x))
                .ToList();
        }

        public void LogUsefulInformation(IAppContext appContext)
        {
            this.Log().Info("");
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
            this.Log().Info("");
        }
    }
}
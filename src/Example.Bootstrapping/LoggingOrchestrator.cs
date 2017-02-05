using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Example.Bootstrapping
{
    public class ConsoleAndFileLogger : CompositeLog<ConsoleAndFileLogger>
    {
        public ConsoleAndFileLogger() : base(new ConsoleLog(), new FileLog())
        {
            
        }
    }

    public class LoggingOrchestrator
    {
        public void InitializeLogging(string mainThreadName, string banner)
        {
            // Set the main thread's name to make it clear in the logs.
            if (Thread.CurrentThread.Name != mainThreadName)
                Thread.CurrentThread.Name = mainThreadName;

            Console.OutputEncoding = Encoding.UTF8;

            // Sets my logger to the console, which goes to the debug output.
            Log.InitializeWith<ConsoleAndFileLogger>();

            this.Log().Debug("Logging initialized.");

            // Show a banner to easily pick out where new instances start
            // in the log file. Plus it just looks cool.
            banner.Split(new[] {Environment.NewLine}, StringSplitOptions.None)
                .Do(x => this.Log().Info(x))
                .ToList();
        }

        public void LogUsefulInformation(IEnvironmentFacade environment, AppSettings appSettings)
        {
            this.Log().Debug("Gathering system information...");
            var assemblyLocation = environment.GetAssemblyLocation();
            var productName = environment.GetProductName();
            var assemblyVersion = environment.GetAssemblyVersion();
            var fileVersion = environment.GetAssemblyFileVersion();
            var productVersion = environment.GetProductVersion();
            var principalName = environment.GetPrincipalName();
            var culture = environment.GetCurrentCulture();
            var hostName = environment.GetHostName();
            var ipAddress = environment.GetCurrentIpV4Address();
            var instanceName = environment.GetServiceInstanceName().IfNullOrEmpty("[Running in console mode]");
            var windowsVersion = environment.GetWindowsVersionName();

            var keyValues = new Dictionary<string, string>
            {
                ["Assembly location"] = assemblyLocation,
                ["Assembly version"] = assemblyVersion,
                ["File version"] = fileVersion,
                ["Product version"] = productVersion,
                ["Instance name"] = instanceName,
                ["Running as"] = $"{principalName} ({culture})",
                ["Network host"] = $"{hostName} ({ipAddress})",
                ["Windows version"] = windowsVersion,
                ["Configuration"] = "=====================",
            };

            appSettings.GetType().GetProperties()
                .ForEach(x => keyValues.Add(x.Name, x.GetValue(appSettings)?.ToString() ?? "[NULL]"));

            var longestKey = keyValues.Keys.Max(x => x.Length);

            this.Log().Info("");
            foreach (var keyValue in keyValues)
            {
                this.Log().Info($"{keyValue.Key.PadLeft(longestKey)}: {keyValue.Value}");
            }
            this.Log().Info("");
            this.Log().Info($"Starting {productName} v{productVersion}");
            this.Log().Info("");
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.Threading;

namespace Example.Bootstrapping
{
    public interface IEnvironmentFacade
    {
        string GetAssemblyLocation();
        string GetAssemblyVersion();
        string GetAssemblyFileVersion();
        string GetProductVersion();
        string GetServiceInstanceName();
        string GetPrincipalName();
        string GetHostName();
        string GetWindowsVersionName();
        IPAddress GetCurrentIpV4Address();
        string GetCurrentCulture();
    }

    public class EnvironmentFacade : IEnvironmentFacade
    {
        private readonly Assembly _applicationAssembly;
        private readonly ConcurrentDictionary<string, object> _cache;

        public EnvironmentFacade(Assembly applicationAssembly)
        {
            _applicationAssembly = applicationAssembly;
            _cache = new ConcurrentDictionary<string, object>();
        }

        public string GetAssemblyLocation()
        {
            return (string)_cache.GetOrAdd("assembly-location", key =>
            {
                var codeBase = _applicationAssembly.CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                var assemblyLocation = Path.GetDirectoryName(path);
                return assemblyLocation;
            });
        }

        public string GetAssemblyVersion()
        {
            return (string) _cache.GetOrAdd("assembly-version", key =>
            {
                var assemblyVersion = _applicationAssembly.GetName().Version.ToString();
                return assemblyVersion;
            });
        }

        public string GetAssemblyFileVersion()
        {
            return (string) _cache.GetOrAdd("assembly-file-version", key =>
            {
                var fileVersion = FileVersionInfo.GetVersionInfo(_applicationAssembly.Location).FileVersion;
                return fileVersion;
            });
        }

        public string GetProductVersion()
        {
            return (string) _cache.GetOrAdd("product-version", key =>
            {
                var productVersion = FileVersionInfo.GetVersionInfo(_applicationAssembly.Location).ProductVersion;
                return productVersion;
            });
        }

        public string GetServiceInstanceName()
        {
            return (string) _cache.GetOrAdd("service-instance-name", key =>
            {
                // Calling System.ServiceProcess.ServiceBase::ServiceNamea allways returns
                // an empty string,
                // see https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=387024

                // So we have to do some more work to find out our service name, this only works if
                // the process contains a single service, if there are more than one services hosted
                // in the process you will have to do something else

                int processId = Process.GetCurrentProcess().Id;
                String query = $"SELECT * FROM Win32_Service where ProcessId = {processId}";
                System.Management.ManagementObjectSearcher searcher =
                    new System.Management.ManagementObjectSearcher(query);

                foreach (System.Management.ManagementObject queryObj in searcher.Get())
                {
                    return queryObj["Name"].ToString();
                }

                return String.Empty;
            });
        }

        public string GetPrincipalName()
        {
            var principalName = WindowsIdentity.GetCurrent().Name;
            return principalName;
        }

        public string GetCurrentCulture()
        {
            var cultureName = Thread.CurrentThread.CurrentCulture.Name;
            return cultureName;
        }

        public string GetHostName()
        {
            var hostName = Environment.MachineName;
            return hostName;
        }

        public string GetWindowsVersionName()
        {
            return (string) _cache.GetOrAdd("windows-version-name", key =>
            {
                var windowsVersion =
                    $"{Environment.OSVersion} {(Environment.Is64BitOperatingSystem ? "64bit" : "32bit")}";
                return windowsVersion;
            });
        }

        public IPAddress GetCurrentIpV4Address()
        {
            var ipAddress =
                Dns.GetHostEntry(Dns.GetHostName())
                    .AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return ipAddress;
        }
    }
}
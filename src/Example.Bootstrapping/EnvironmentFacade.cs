using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;

namespace Example.Bootstrapping
{
    public interface IEnvironmentFacade
    {
        string GetAssemblyLocation();
        Version GetAssemblyVersion();
        Version GetAssemblyFileVersion();
        string GetServiceInstanceName();
        string GetPrincipalName();
        string GetHostName();
        string GetWindowsVersionName();
        IPAddress GetCurrentIpV4Address();
    }

    public class EnvironmentFacade : IEnvironmentFacade
    {
        private readonly Assembly _applicationAssembly;

        public EnvironmentFacade(Assembly applicationAssembly)
        {
            _applicationAssembly = applicationAssembly;
        }

        public string GetAssemblyLocation()
        {
            var codeBase = _applicationAssembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var assemblyLocation = Path.GetDirectoryName(path);
            return assemblyLocation;
        }

        public Version GetAssemblyVersion()
        {
            var assemblyVersion = _applicationAssembly.GetName().Version;
            return assemblyVersion;
        }

        public Version GetAssemblyFileVersion()
        {
            var fileVersion = new Version(FileVersionInfo.GetVersionInfo(_applicationAssembly.Location).FileVersion);
            return fileVersion;
        }

        public string GetServiceInstanceName()
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
        }

        public string GetPrincipalName()
        {
            var principalName = WindowsIdentity.GetCurrent().Name;
            return principalName;
        }

        public string GetHostName()
        {
            var hostName = Environment.MachineName;
            return hostName;
        }

        public string GetWindowsVersionName()
        {
            var windowsVersion = $"{Environment.OSVersion} {(Environment.Is64BitOperatingSystem ? "64bit" : "32bit")}";
            return windowsVersion;
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
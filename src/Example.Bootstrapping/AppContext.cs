using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;

namespace Example.Bootstrapping
{
    public interface IAppContext
    {
        string AppId { get; }
        string LongName { get; }
        string AssemblyLocation { get; }
        Version AssemblyVersion { get; }
        Version AssemblyFileVersion { get; }
        string PrincipalName { get; }
        string HostName { get; }
        string InstanceName { get; }
    }

    public class AppContext : IAppContext
    {
        public AppContext(
            string appId, string longName, string assemblyLocation, 
            Version assemblyVersion, Version assemblyFileVersion, 
            string principalName, string hostName, string instanceName)
        {
            AppId = appId;
            LongName = longName;
            AssemblyLocation = assemblyLocation;
            AssemblyVersion = assemblyVersion;
            AssemblyFileVersion = assemblyFileVersion;
            PrincipalName = principalName;
            HostName = hostName;
            InstanceName = instanceName;
        }

        public string AppId { get; }
        public string LongName { get; }
        public string AssemblyLocation { get; }
        public Version AssemblyVersion { get; }
        public Version AssemblyFileVersion { get; }
        public string PrincipalName { get; }
        public string HostName { get; }
        public string InstanceName { get; }
    }

    public static class AppContextService
    {
        public static IAppContext GatherAppContext(string appId, string longName, Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var assemblyLocation = Path.GetDirectoryName(path);

            var assemblyVersion = assembly.GetName().Version;
            var fileVersion = new Version(FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion);

            var principalName = WindowsIdentity.GetCurrent().Name;
            var hostName = Environment.MachineName;

            var instanceName = GetServiceName();

            return new AppContext(appId, longName, assemblyLocation, assemblyVersion, fileVersion, principalName, hostName, instanceName);
        }
        
        private static string GetServiceName()
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
    }
}
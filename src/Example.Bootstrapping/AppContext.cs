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
            var environment = new EnvironmentFacade(assembly);
            
            var assemblyLocation = environment.GetAssemblyLocation();
            var assemblyVersion = environment.GetAssemblyVersion();
            var fileVersion = environment.GetAssemblyFileVersion();
            var principalName = environment.GetPrincipalName();
            var hostName = environment.GetHostName();
            var instanceName = environment.GetServiceInstanceName();

            return new AppContext(appId, longName, assemblyLocation, assemblyVersion, fileVersion, principalName, hostName, instanceName);
        }
    }
}
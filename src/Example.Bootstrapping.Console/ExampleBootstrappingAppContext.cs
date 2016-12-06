using System;

namespace Example.Bootstrapping.Console
{
    public class ExampleBootstrappingAppContext : IAppContext
    {
        public ExampleBootstrappingAppContext(
            string hostName, string instanceName, string principalName,
            string assemblyLocation, Version assemblyVersion, Version assemblyFileVersion)
        {
            HostName = hostName;
            InstanceName = instanceName;
            AssemblyLocation = assemblyLocation;
            AssemblyVersion = assemblyVersion;
            AssemblyFileVersion = assemblyFileVersion;
            PrincipalName = principalName;
        }

        public string LongName => "Example Bootstrapping Console App";
        public string AppId => "bootstrapping-console";
        public string AssemblyLocation { get; }
        public string PrincipalName { get; }
        public string HostName { get; }
        public string InstanceName { get; }
        public Version AssemblyVersion { get; }
        public Version AssemblyFileVersion { get; }
    }
}
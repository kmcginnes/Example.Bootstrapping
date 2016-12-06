using System;

namespace Example.Bootstrapping
{
    public interface IAppContext
    {
        string LongName { get; }
        string AppId { get; }
        string AssemblyLocation { get; }
        Version AssemblyVersion { get; }
        Version AssemblyFileVersion { get; }
        string PrincipalName { get; }
        string HostName { get; }
        string InstanceName { get; }
    }
}
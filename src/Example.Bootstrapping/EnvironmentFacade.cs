using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Example.Bootstrapping
{
    public interface IEnvironmentFacade
    {
        string GetAssemblyLocation();
        string GetProductName();
        string GetAssemblyVersion();
        string GetAssemblyFileVersion();
        string GetProductVersion();
        string GetServiceInstanceName();
        string GetPrincipalName();
        string GetCurrentCulture();
        string GetHostName();
        string GetWindowsVersionName();
        IPAddress GetCurrentIpV4Address();
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
                StringBuilder sb = UnsafeNativeMethods.GetModuleFileNameLongPath(UnsafeNativeMethods.NullHandleRef);
                var assemblyLocation = Path.GetDirectoryName(sb.ToString());
                return assemblyLocation;
            });
        }

        public string GetProductName()
        {
            return (string) _cache.GetOrAdd("product-name", key =>
            {
                string productName = null;

                // custom attribute
                //
                var attrs = _applicationAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attrs.Length > 0)
                {
                    productName = ((AssemblyProductAttribute)attrs[0]).Product;
                }

                // win32 version info
                //
                if (String.IsNullOrEmpty(productName))
                {
                    productName = GetAppFileVersionInfo().ProductName;
                    productName = productName?.Trim();
                }
                return productName;
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
                var fileVersion = GetAppFileVersionInfo().FileVersion;
                return fileVersion;
            });
        }

        public string GetProductVersion()
        {
            return (string) _cache.GetOrAdd("product-version", key =>
            {
                var attrs = _applicationAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                var productVersion = attrs.Length > 0
                    ? ((AssemblyInformationalVersionAttribute) attrs[0]).InformationalVersion
                    : String.Empty;
                
                if (String.IsNullOrEmpty(productVersion))
                {
                    productVersion = GetAppFileVersionInfo().ProductVersion;
                    productVersion = productVersion?.Trim() ?? String.Empty;
                }

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

        // ** Helpers ** 

        private FileVersionInfo GetAppFileVersionInfo()
        {
            var appFileVersion = FileVersionInfo.GetVersionInfo(_applicationAssembly.Location);
            return appFileVersion;
        }

        private static class UnsafeNativeMethods
        {
            public static readonly HandleRef NullHandleRef = new HandleRef((object)null, IntPtr.Zero);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern int GetModuleFileName(HandleRef hModule, StringBuilder buffer, int length);

            public static StringBuilder GetModuleFileNameLongPath(HandleRef hModule)
            {
                StringBuilder buffer = new StringBuilder(260);
                int num = 1;
                int moduleFileName;
                while ((moduleFileName = GetModuleFileName(hModule, buffer, buffer.Capacity)) == buffer.Capacity && Marshal.GetLastWin32Error() == 122 && buffer.Capacity < (int)short.MaxValue)
                {
                    num += 2;
                    int capacity = num * 260 < (int)short.MaxValue ? num * 260 : (int)short.MaxValue;
                    buffer.EnsureCapacity(capacity);
                }
                buffer.Length = moduleFileName;
                return buffer;
            }
        }
    }
}
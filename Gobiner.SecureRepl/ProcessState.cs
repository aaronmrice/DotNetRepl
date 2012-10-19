using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Gobiner.SecureRepl.Extensions;

namespace Gobiner.SecureRepl
{
    public static class ProcessState
    {
        static ProcessState()
        {
            var safePerms = new PermissionSet(PermissionState.None);
            safePerms.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            safePerms.AddPermission(new SecurityPermission(SecurityPermissionFlag.SerializationFormatter));
            safePerms.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));
            safePerms.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, Assembly.GetExecutingAssembly().Location));

            var safeDomain = AppDomain.CreateDomain(
                "Gobiner.SecureRepl.UnsafeProgram",
                AppDomain.CurrentDomain.Evidence,
                new AppDomainSetup() { DisallowCodeDownload = true, ApplicationBase = AppDomain.CurrentDomain.BaseDirectory },
                safePerms,
                Assembly.GetExecutingAssembly().GetStrongName(),
                typeof(SecureRoslynWrapper).Assembly.GetStrongName());

            SecureRepl = (SecureRoslynWrapper)safeDomain.CreateInstanceFromAndUnwrap(typeof(SecureRoslynWrapper).Assembly.Location, typeof(SecureRoslynWrapper).FullName);
        }

        internal static SecureRoslynWrapper SecureRepl { get; private set; }
        internal static DateTime LastHeartBeat { get; set; }
        internal static bool InstructedToDie { get; set; }
    }
}

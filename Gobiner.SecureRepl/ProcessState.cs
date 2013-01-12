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
    internal static class ProcessState
    {
        private static object lockObject = new object();

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

        private static SecureRoslynWrapper _secureRepl;
        internal static SecureRoslynWrapper SecureRepl
        {
            get
            {
                lock (lockObject)
                {
                    return _secureRepl;
                }
            }
            private set
            {
                lock (lockObject)
                {
                    _secureRepl = value;
                }
            }
        }

        private static DateTime _lastHeartBeat;
        internal static DateTime LastHeartBeat
        {
            get
            {
                lock (lockObject)
                {
                    return _lastHeartBeat;
                }
            }
            set
            {
                lock (lockObject)
                {
                    _lastHeartBeat = value;
                }
            }
        }

        private static bool _instructedToDie;
        internal static bool InstructedToDie
        {
            get
            {
                lock (lockObject)
                {
                    return _instructedToDie;
                }
            }
            set
            {
                lock (lockObject)
                {
                    _instructedToDie = value;
                }
            }
        }
    }
}

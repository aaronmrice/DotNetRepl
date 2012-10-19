using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Gobiner.SecureRepl.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Create a StrongName that matches a specific assembly
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="assembly"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// if <paramref name="assembly"/> does not represent a strongly named assembly
        /// </exception>
        /// <param name="assembly">Assembly to create a StrongName for</param>
        /// <returns>A StrongName that matches the given assembly</returns>
        public static StrongName GetStrongName(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            AssemblyName assemblyName = assembly.GetName();
            Debug.Assert(assemblyName != null, "Could not get assembly name");

            // get the public key blob
            byte[] publicKey = assemblyName.GetPublicKey();
            if (publicKey == null || publicKey.Length == 0)
                throw new InvalidOperationException("Assembly is not strongly named");

            StrongNamePublicKeyBlob keyBlob = new StrongNamePublicKeyBlob(publicKey);

            // and create the StrongName
            return new StrongName(keyBlob, assemblyName.Name, assemblyName.Version);
        }
    }
}

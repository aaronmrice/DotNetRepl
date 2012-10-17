using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Diagnostics;
using System.Dynamic;

namespace Gobiner.SecureRepl
{
	class Program
	{
		private static string[] references = new string[] { "System.dll", "System.Core.dll", "System.Data.dll", "System.Data.DataSetExtensions.dll", "Microsoft.CSharp.dll", "System.Xml.dll", "System.Xml.Linq.dll", "System.Data.Entity.dll", "System.Windows.Forms.dll", };
		private static string[] namespaces = new string[] { "System", "System.Collections.Generic", "System.Linq", "System.Text" };

        [SecurityCritical]
        static void Main(string[] args)
		{
			var safePerms = new PermissionSet(PermissionState.None);
			safePerms.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
			safePerms.AddPermission(new SecurityPermission(SecurityPermissionFlag.SerializationFormatter));
			safePerms.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));
            safePerms.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, Assembly.GetExecutingAssembly().Location));
            safePerms.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, typeof(ReplTester).Assembly.Location));
			safePerms.AddPermission(new UIPermission(PermissionState.Unrestricted)); // required to run an .exe

            var safeDomain = AppDomain.CreateDomain(
                "Gobiner.SecureRepl.UnsafeProgram",
                AppDomain.CurrentDomain.Evidence,
                new AppDomainSetup() { DisallowCodeDownload = true, ApplicationBase = AppDomain.CurrentDomain.BaseDirectory },
                safePerms,
                GetStrongName(Assembly.GetExecutingAssembly()),
                GetStrongName(typeof(ReplTester).Assembly));


            var test = (ReplTester)safeDomain.CreateInstanceFromAndUnwrap(typeof(ReplTester).Assembly.Location, typeof(ReplTester).FullName);
            test = new ReplTester();
			var result = test.DoIt();
            Console.WriteLine(result);
        }

		public class ReplTester : MarshalByRefObject
		{
            [SecurityCritical]
			public string DoIt()
			{
                new PermissionSet(PermissionState.Unrestricted).Assert();

                var engine = new ScriptEngine();
                foreach (var assembly in references)
                    engine.AddReference(assembly);
                foreach (var name in namespaces)
                    engine.ImportNamespace(name);

                var session = engine.CreateSession();

                var submission = session.CompileSubmission<object>("Tuple.Create(4,20)");

                CodeAccessPermission.RevertAssert();

                var result = submission.Execute();
                var ret = ObjectFormatter.Instance.FormatObject(result);

                // Required to work around full-trust finalizers in Roslyn.Compilers.MetadataReader.MemoryMappedFile
                new PermissionSet(PermissionState.Unrestricted).Assert();
                GC.Collect(3, GCCollectionMode.Forced, true);
                CodeAccessPermission.RevertAssert();

                return ret;
			}
		}

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
		private static StrongName GetStrongName(Assembly assembly)
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

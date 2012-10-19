using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Roslyn.Compilers;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace Gobiner.SecureRepl
{
    class SecureRoslynWrapper : MarshalByRefObject
    {
        private static readonly string[] references = new string[] { "System.dll", "System.Core.dll", "System.Data.dll", "System.Data.DataSetExtensions.dll", "Microsoft.CSharp.dll", "System.Xml.dll", "System.Xml.Linq.dll", "System.Data.Entity.dll", "System.Windows.Forms.dll", };
        private static readonly string[] namespaces = new string[] { "System", "System.Collections.Generic", "System.Linq", "System.Text" };

        private ScriptEngine Engine;
        private Session Session;

        [SecurityCritical]
        public SecureRoslynWrapper()
        {
            new PermissionSet(PermissionState.Unrestricted).Assert();
            Engine = new ScriptEngine();
            foreach (var assembly in references)
                Engine.AddReference(assembly);
            foreach (var name in namespaces)
                Engine.ImportNamespace(name);

            Session = Engine.CreateSession();
            CodeAccessPermission.RevertAssert();
        }

        [SecurityCritical]
        public string Execute(string inp)
        {
            new PermissionSet(PermissionState.Unrestricted).Assert();

            string ret = null;
            try
            {
                var submission = Session.CompileSubmission<object>(inp);

                CodeAccessPermission.RevertAssert();

                var result = submission.Execute();
                ret = ObjectFormatter.Instance.FormatObject(result);
            }
            catch (CompilationErrorException ex)
            {
                ret = ex.Message;
            }
            catch (Exception ex)
            {
                ret = ex.ToString();
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }


            // Required to work around full-trust finalizers in Roslyn.Compilers.MetadataReader.MemoryMappedFile
            new PermissionSet(PermissionState.Unrestricted).Assert();
            GC.Collect(3, GCCollectionMode.Forced, true);
            CodeAccessPermission.RevertAssert();

            return ret;
        }
    }
}

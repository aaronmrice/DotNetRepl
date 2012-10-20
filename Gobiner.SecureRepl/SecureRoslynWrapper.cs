using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Roslyn.Compilers;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace Gobiner.SecureRepl
{
    internal class SecureRoslynWrapper : MarshalByRefObject
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
        public ReplResult Execute(string inp)
        {
            new PermissionSet(PermissionState.Unrestricted).Assert();

            ReplResult ret = new ReplResult();
            try
            {
                var submission = Session.CompileSubmission<object>(inp);

                var thread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        var result = submission.Execute();
                        ret.ExecutionResult = ObjectFormatter.Instance.FormatObject(result);
                    }
                    catch (Exception ex)
                    {
                        ret.ExecutionResult = ex.ToString();
                    }
                }), 1024 * 1024);
                thread.Start();
                if (!thread.Join(4000))
                {
                    thread.Abort();
                }
                if (!thread.Join(500))
                {
                    // Some kind of submission that can't be aborted, process needs to die
                    ret.ExecutionStillOngoing = true;
                }
            }
            catch (CompilationErrorException ex)
            {
                ret.CompilationErrors = ex.Diagnostics.Select(x => x.Info.ToString()).ToArray();
            }
            catch (Exception ex)
            {
                ret.ExecutionResult = ex.ToString();
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

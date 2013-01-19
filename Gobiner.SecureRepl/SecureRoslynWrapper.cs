using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private object lockObject = new object();
        private Submission<object> submission;
        private ReplResult result;

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

            result = new ReplResult();
            lock (lockObject)
            {
                try
                {
                    submission = Session.CompileSubmission<object>(inp);

                    var thread = new Thread(new ParameterizedThreadStart(ExecuteSubmission), 1024 * 1024);
                    thread.Start(submission);
                    if (!thread.Join(4000))
                    {
                        thread.Abort();
                    }
                    if (!thread.Join(500))
                    {
                        // Some kind of submission that can't be aborted, process needs to die
                        result.ExecutionStillOngoing = true;
                    }
                }
                catch (CompilationErrorException ex)
                {
                    result.CompilationErrors = ex.Diagnostics.Select(x => x.Info.ToString()).ToArray();
                }
                catch (Exception ex)
                {
                    result.ExecutionResult = ex.ToString();
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
            }

            return result;
        }

        [SecurityCritical]
        private void ExecuteSubmission(object parameter)
        {
            Submission<object> submission = (Submission<object>)parameter;
            AppDomain.CurrentDomain.PermissionSet.PermitOnly();
            try
            {
                var execution = submission.Execute();
                result.ExecutionResult = ObjectFormatter.Instance.FormatObject(execution);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(SecurityException))
                    new PermissionSet(PermissionState.Unrestricted).Assert();

                result.ExecutionResult = ex.ToString();
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
        }
    }
}

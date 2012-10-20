using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Gobiner.SecureRepl
{
    public class Repl : IRepl
    {
        [SecurityCritical]
        public string Execute(string inp)
        {
            KeepAlive();
            var result = ProcessState.SecureRepl.Execute(inp);
            if (result.ExecutionStillOngoing)
                ProcessState.InstructedToDie = true;
            if (result.CompilationErrors != null)
                return result.CompilationErrors[0];
            return result.ExecutionResult;
        }

        public void Kill()
        {
            ProcessState.InstructedToDie = true;
        }

        public void KeepAlive()
        {
            ProcessState.LastHeartBeat = DateTime.Now;
        }
    }
}

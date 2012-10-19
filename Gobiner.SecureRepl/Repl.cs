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
            return ProcessState.SecureRepl.Execute(inp);
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

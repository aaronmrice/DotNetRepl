using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobiner.SecureRepl
{
    internal class ReplResult : MarshalByRefObject
    {
        public string[] CompilationErrors;
        public string ExecutionResult;
        public bool ExecutionStillOngoing;
    }
}

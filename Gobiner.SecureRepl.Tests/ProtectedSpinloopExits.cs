using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gobiner.SecureRepl.Tests
{
    public class ProtectedSpinloopExits
    {
        [Fact(Timeout = 10000)]
        public void Test()
        {
            var repl = new Repl();
            repl.Execute(@"try { while(true); } finally { while(true); }");
            Assert.True(true);
        }
    }
}

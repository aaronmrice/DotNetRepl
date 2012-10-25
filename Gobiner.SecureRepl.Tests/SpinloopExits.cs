using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gobiner.SecureRepl.Tests
{
    public class SpinloopExits
    {
        [Fact(Timeout = 10000)]
        public void Test()
        {
            var repl = new Repl();
            repl.Execute(@"while(true);");
            Assert.True(true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gobiner.SecureRepl.Tests
{
    public class ReplTests
    {
        [Fact(Timeout = 10000)]
        public void ProtectedSpinloopReturns()
        {
            var repl = new Repl();
            repl.Execute(@"try { while(true); } finally { while(true); }");
            Assert.True(true);
        }

        [Fact(Timeout = 10000)]
        public void SpinloopReturns()
        {
            var repl = new Repl();
            repl.Execute(@"while(true);");
            Assert.True(true);
        }

        [Fact]
        public void CantWriteFile()
        {
            if (File.Exists("c:\\test.txt"))
                File.Delete("c:\\test.txt");

            var repl = new Repl();
            repl.Execute(@"System.IO.File.WriteAllText(@""c:\test.txt"", ""test"");");
            Assert.False(File.Exists("c:\\test.txt"));

            if (File.Exists("c:\\test.txt"))
                File.Delete("c:\\test.txt");
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gobiner.SecureRepl.Tests
{
    public class ReplTests : IDisposable
    {
        [Fact(Timeout = 10000)]
        public void ProtectedSpinloopReturns()
        {
            using (var repl = new ProcessWrapper())
            {
            repl.Execute(@"try { while(true); } finally { while(true); }");
            Assert.True(true);
            Assert.False(repl.IsProcessAlive);
            }
        }

        [Fact(Timeout = 10000)]
        public void SpinloopReturns()
        {
            using (var repl = new ProcessWrapper())
            {
                repl.Execute(@"while(true);");
                Assert.True(true);
                Assert.False(repl.IsProcessAlive);
            }
        }

        [Fact]
        public void CantWriteFile()
        {
            if (File.Exists("c:\\test.txt"))
                File.Delete("c:\\test.txt");

            using (var repl = new ProcessWrapper())
            {
                repl.Execute(@"System.IO.File.WriteAllText(@""c:\test.txt"", ""test"");");
                repl.Kill();
            
            Assert.False(File.Exists("c:\\test.txt"));
            Assert.False(repl.IsProcessAlive);
            }
        }

        [Fact]
        public void CantAssertNewPermissions()
        {
            if (File.Exists("c:\\test.txt"))
                File.Delete("c:\\test.txt");

            using (var repl = new ProcessWrapper())
            {
                repl.Execute(@"new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert(); System.IO.File.WriteAllText(@""c:\test.txt"", ""test"");");
                repl.Kill();
                Assert.False(File.Exists("c:\\test.txt"));
                Assert.False(repl.IsProcessAlive);
            }
        }

        public void Dispose()
        {
            if (File.Exists("c:\\test.txt"))
                File.Delete("c:\\test.txt");
        }
    }
}

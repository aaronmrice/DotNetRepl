using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            }
        }

        [Fact(Timeout = 20000)]
        public void ProtectedSpinloopKillsProcess()
        {
            using (var repl = new ProcessWrapper())
            {
                repl.Execute(@"try { while(true); } finally { while(true); }");
                while (repl.IsProcessAlive)
                    Thread.Sleep(500);
                Assert.True(true);
            }
        }

        [Fact(Timeout = 10000)]
        public void SpinloopReturns()
        {
            using (var repl = new ProcessWrapper())
            {
                repl.Execute(@"while(true);");
                Assert.True(true);
            }
        }

        [Fact(Timeout = 20000)]
        public void SpinloopDoesNotKillProcess()
        {
            using (var repl = new ProcessWrapper())
            {
                repl.Execute(@"while(true);");
                Thread.Sleep(10000);
                Assert.True(repl.IsProcessAlive);
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
            }
        }

        public void Dispose()
        {
            if (File.Exists("c:\\test.txt"))
                File.Delete("c:\\test.txt");
        }
    }
}

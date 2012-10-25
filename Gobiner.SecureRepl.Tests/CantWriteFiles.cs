using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gobiner.SecureRepl.Tests
{
    public class CantWriteFiles : IDisposable
    {
        public CantWriteFiles()
        {
            if (File.Exists("c:\\test.txt"))
                File.Delete("c:\\test.txt");
        }

        [Fact]
        public void Test()
        {
            var repl = new Repl();
            repl.Execute(@"System.IO.File.WriteAllText(@""c:\test.txt"", ""test"");");
            Assert.False(File.Exists("c:\\test.txt"));
        }

        public void Dispose()
        {
            if (File.Exists("c:\\test.txt"))
                File.Delete("c:\\test.txt");
        }
    }
}

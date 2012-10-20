using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gobiner.SecureRepl.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = new Random().Next(10000, 20000);
            Process.Start("Gobiner.SecureRepl.exe", port.ToString());
            Thread.Sleep(500);
            var wcfClient = new Wcf.ReplClient("BasicHttpBinding_IRepl", "http://localhost:" + port + "/Repl");

            var input = "";

            while(true)
            {
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    break;

                Console.WriteLine(wcfClient.Execute(input));
            }

            wcfClient.Kill();
        }
    }
}

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
            var repl = new Gobiner.SecureRepl.ProcessWrapper();
            var input = "";

            while(true)
            {
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    break;

                Console.WriteLine(repl.Execute(input));
            }

            repl.Kill();
        }
    }
}

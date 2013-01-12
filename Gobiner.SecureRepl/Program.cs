using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Diagnostics;
using System.Dynamic;
using Gobiner.SecureRepl.Extensions;
using System.ServiceModel;

namespace Gobiner.SecureRepl
{
    class Program
    {
        [SecurityCritical]
        static void Main(string[] args)
        {
            try
            {
                int port = 0;
                if (args.Length == 0 || !int.TryParse(args[0], out port))
                {
                    Console.Error.WriteLine("You must pass in an argument for the port.");
                    Environment.Exit(1);
                }

                ProcessState.LastHeartBeat = DateTime.Now;

                using (ServiceHost host = new ServiceHost(typeof(Repl), new Uri("net.tcp://localhost:" + port)))
                {
                    host.AddServiceEndpoint(typeof(IRepl), new NetTcpBinding(), "net.tcp://localhost:" + port + "/Repl");

                    host.Open();

                    Console.WriteLine("Service is available.");
                    while (ProcessState.LastHeartBeat + new TimeSpan(0, 5, 0) > DateTime.Now && !ProcessState.InstructedToDie)
                    {
                        Thread.Sleep(5000);
                    }

                    host.Close();
                }

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Assembly.GetExecutingAssembly().Location + ".exception", ex.ToString());
                throw;
            }
        }
    }
}

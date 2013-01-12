using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gobiner.SecureRepl
{
    public class ProcessWrapper : IRepl, IDisposable
    {
        private readonly int port = new Random().Next(10000, 20000);
        private Lazy<Wcf.ReplClient> remoteRepl;
        private Process process;

        public ProcessWrapper()
        {
            remoteRepl = new Lazy<Wcf.ReplClient>(() =>
            {
                process = Process.Start(new ProcessStartInfo("Gobiner.SecureRepl.exe", port.ToString()) { CreateNoWindow = true });
                Thread.Sleep(500);
                var wcfClient = new Wcf.ReplClient(new System.ServiceModel.NetTcpBinding(), new System.ServiceModel.EndpointAddress("net.tcp://localhost:" + port + "/Repl"));
                return wcfClient;
            });
        }

        public bool ProcessHasStarted
        {
            get
            {
                return remoteRepl.IsValueCreated;
            }
        }

        public bool IsProcessAlive
        {
            get
            {
                return ProcessHasStarted && !process.HasExited;
            }
        }

        public string Execute(string inp)
        {
            return remoteRepl.Value.Execute(inp);
        }

        public void Kill()
        {
            remoteRepl.Value.Kill();
        }

        public void KeepAlive()
        {
            remoteRepl.Value.KeepAlive();
        }

        public void Dispose()
        {
            Dispose(true);
        }
        ~ProcessWrapper()
        {
            Dispose(false);
        }
        private void Dispose(bool disposing)
        {
            if (remoteRepl != null && remoteRepl.IsValueCreated && remoteRepl.Value != null)
                remoteRepl.Value.Kill();

            if (disposing)
            {
                remoteRepl.Value.Kill();
                remoteRepl.Value.Abort();
            }

            if (process != null)
                process.Kill();

            remoteRepl = null;
        }
    }
}

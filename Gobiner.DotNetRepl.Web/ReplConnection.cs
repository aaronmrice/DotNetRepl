using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Gobiner.DotNetRepl.Web
{
    public class ReplConnection : IDisposable
    {
        public static Dictionary<Guid, ReplConnection> Connections { get; private set; }
        private static readonly Random Random = new Random();
        static ReplConnection()
        {
            Connections = new Dictionary<Guid, ReplConnection>();
        }

        private Wcf.ReplClient _wcfClient;
        private Wcf.ReplClient WcfClient
        {
            get
            {
                if (_wcfClient == null)
                    Initialize();
                return _wcfClient;
            }
        }

        public Guid ID { get; set; }

        private int Port { get; set; }
        private int ProcessID { get; set; }

        private void Initialize()
        {
            var unavailablePorts = new HashSet<int>(Connections.Values.Select(x => x.Port));
            do
            {
                Port = Random.Next(10000, 20000);
            } while (unavailablePorts.Contains(Port));

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo(HttpContext.Current.Server.MapPath("~/bin/Gobiner.SecureRepl.exe"), Port.ToString()),
            };
            process.Start();
            ProcessID = process.Id;
            
            _wcfClient = new Wcf.ReplClient("BasicHttpBinding_IRepl", "http://localhost:" + Port + "/Repl");
        }

        public ReplConnection(Guid id)
        {
            ID = id;
            Connections[id] = this;
        }

        public void Kill()
        {
            WcfClient.Kill();
        }
        public void KeepAlive()
        {
            WcfClient.KeepAlive();
        }
        public string Execute(string inp)
        {
            return WcfClient.Execute(inp);
        }

        public void Dispose()
        {
            Dispose(true);
        }
        ~ReplConnection()
        {
            Dispose(false);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _wcfClient.Abort();
            }
            if (_wcfClient != null)
                WcfClient.Kill();
            _wcfClient = null;
            Connections.Remove(this.ID);
        }
    }
}
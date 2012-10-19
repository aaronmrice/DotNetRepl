using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Gobiner.SecureRepl
{
    [ServiceContract]
    public interface IRepl
    {
        [OperationContract]
        string Execute(string inp);
        [OperationContract]
        void Kill();
        [OperationContract]
        void KeepAlive();
    }
}

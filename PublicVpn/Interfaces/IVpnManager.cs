using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicVpn.Interfaces
{
    public interface IVpnManager
    {
        public bool Connect(IHost host);
        public bool Disconnect();
    }
}

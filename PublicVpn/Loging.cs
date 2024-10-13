using PublicVpn.Interfaces;
using PublicVpn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicVpn
{
    public static class Loging
    {
        public delegate void LogDelegate(Exception ex, IHost host);
        public static event LogDelegate? LogEvent;
        public static void Log(Exception ex, IHost host) => LogEvent?.Invoke(ex, host);
        public static void Log(Exception ex) => LogEvent?.Invoke(ex, new Levpro_ruVpnModel(autoIncrementId: false) { IP = "none", Country = "none", DDNS = "none"});
    }
}

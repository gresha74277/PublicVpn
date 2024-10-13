using PublicVpn.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicVpn.Interfaces
{
    public interface IHost
    {
        public long Id { get; }
        public string IP { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime LastCheack { get; set; }
        public bool IsChecked { get; }
        public StatusEnum Status { get; set; }
        public bool IsStatusChanged { get; set; }
        public int SpeedInBytePerSec { get; set; }
    }
}

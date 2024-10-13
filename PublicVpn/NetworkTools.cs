using PublicVpn.Enums;
using PublicVpn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PublicVpn
{
    public class NetworkTools
    {
        readonly string speedTest1mb = "http://cachefly.cachefly.net/1mb.test";
        readonly string speedTest10mb = "http://cachefly.cachefly.net/10mb.test";
        readonly string speedTest100mb = "http://cachefly.cachefly.net/100mb.test";
        readonly static string[] globalPublicHosts = new[] { "google.com", "ya.ru" };

        public static Task<bool> IsOnline()
        {
            return Task.Factory.StartNew(() =>
            {
                using (var ping = new Ping())
                {
                    foreach (var host in globalPublicHosts)
                    {
                        try
                        {
                            var reply = ping.Send(host);
                            if (reply.Status == IPStatus.Success)
                            {
                                return true;
                            }
                        }
                        catch { continue; }
                    }
                }
                return false;
            });
        }

    }
}

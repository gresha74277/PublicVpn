using Newtonsoft.Json;
using PublicVpn.Enums;
using PublicVpn.Interfaces;
using System.Net.NetworkInformation;

namespace PublicVpn.Models
{
    public record Levpro_ruVpnModel : IHost
    {
        public static long LastId { get; set; } = 0;
        //https://levpro.ru/uploads/vpn.json?v3
        //{
        //"country":"\u0421\u043e\u0435\u0434\u0438\u043d\u0435\u043d\u043d\u044b\u0435 \u0428\u0442\u0430\u0442\u044b",
        //"ddns":"nhcc.opengw.net",
        //"ip":"75.76.167.162",
        //"uptime":"7 \u0434\u043d\u0435\u0439",
        //"quality":"78.07 \u043c\u0431\u0438\u0442",
        //"ping":"30 \u043c\u0441",
        //"login":"vpn",
        //"password":"vpn"
        //}
        public long Id { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; } = string.Empty;
        [JsonProperty("ddns")]
        public string DDNS { get; set; } = string.Empty;
        [JsonProperty("ip")]
        public string IP { get; set; } = string.Empty;
        [JsonProperty("uptime")]
        public string UpTime { get; set; } = string.Empty;
        [JsonProperty("quality")]
        public string Quality { get; set; } = string.Empty;
        [JsonProperty("ping")]
        public string Ping { get; set; } = string.Empty;
        [JsonProperty("login")]
        public string Login { get; set; } = string.Empty;
        [JsonProperty("password")]
        public string Password { get; set; } = string.Empty;
        public int SpeedInBytePerSec { get; set; } = 0;
        public StatusEnum Status { get; set; } = StatusEnum.Unknown;
        public bool IsStatusChanged { get; set; } = false;
        public bool IsChecked => DateTime.Now - LastCheack < TimeSpan.FromSeconds(30);
        public DateTime LastCheack { get; set; }
        public Levpro_ruVpnModel()
        {
            this.Id = Levpro_ruVpnModel.LastId;
            Levpro_ruVpnModel.LastId++;
            Autoping();
        }
        public Levpro_ruVpnModel(long initLasId = long.MinValue, bool autoIncrementId = true, bool autoping = true)
        {
            if (initLasId != long.MinValue) Levpro_ruVpnModel.LastId = initLasId;
            if (autoIncrementId)
            {
                this.Id = Levpro_ruVpnModel.LastId;
                Levpro_ruVpnModel.LastId++;
            }
            else this.Id = -1;
            if (autoping) Autoping();
        }
        public static async Task<IEnumerable<Levpro_ruVpnModel>?> GetVpnList() => await GetVpnList("https://levpro.ru/uploads/vpn.json?v3");
        public static async Task<IEnumerable<Levpro_ruVpnModel>?> GetVpnList(string url)
        {
            return await Task.Factory.StartNew(() =>
            {
                IEnumerable<Levpro_ruVpnModel>? listVpn = null;
                using (HttpClient httpClient = new())
                {
                    try
                    {
                        var json = httpClient.GetStringAsync(url).Result;
                        listVpn = JsonConvert.DeserializeObject<IEnumerable<Levpro_ruVpnModel>>(json);
                    }
                    catch (Exception ex)
                    {
                        Loging.Log(ex);
                    }
                }
                return listVpn;
            });
        }
        public void StartAutoping()
        {
            if (!autoping) Autoping();
            autoping = true;
        }
        public void StopAutoping() => autoping = false;
        public bool AutopingIsWorking => autoping;
        bool autoping = true;
        void Autoping()
        {
            autoping = true;
            Task.Factory.StartNew(() =>
            {
                while (autoping)
                {
                    try
                    {
                        Thread.Sleep(2000 + new Random().Next(1000, 3000));
                        if (!this.IsChecked)
                        {
                            this.LastCheack = DateTime.Now;
                            using (Ping ping = new())
                            {
                                var reply = ping.Send(this.IP);
                                if (reply != null)
                                {
                                    switch (reply.Status)
                                    {
                                        case IPStatus.Success:
                                            if (this.Status != StatusEnum.Online)
                                            {
                                                this.Status = StatusEnum.Online;
                                                this.IsStatusChanged = true;
                                            }
                                            else this.IsStatusChanged = false;
                                            break;
                                        default:
                                            if (this.Status != StatusEnum.Offline)
                                            {
                                                this.Status = StatusEnum.Offline;
                                                this.IsStatusChanged = true;
                                            }
                                            else this.IsStatusChanged = false;
                                            Loging.Log(new Exception($"Ping result: IP:{this.IP} - {reply.Status}"));
                                            break;
                                    }
                                }
                                else Loging.Log(new Exception($"Ping result: IP:{this.IP} - can't get reply from host"));
                            }
                            if (!autoping) break;
                        }
                    }
                    catch(Exception ex) { Loging.Log(ex, this); }
                }

            });
        }
    }
}

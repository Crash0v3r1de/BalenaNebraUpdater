using BalenaNebraUpdater.Objs;
using BalenaNebraUpdater.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BalenaNebraUpdater.Tools
{
    static class Discord
    {
        public static void SendWebhook() {
            if (!String.IsNullOrEmpty(SettingsStatic.Settings.webhook)) {
                WebhookBasicJSON tmp = new WebhookBasicJSON();
                tmp.avatar_url = "https://991tech.org/cdn-img/winbox.png";
                tmp.content = $"New balena image update (fleet updated) | Hash: {SettingsStatic.Settings.CurrentCommit}";
                tmp.username = "Balena Helium-Rak Auto Updater";

                var web = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(tmp), Encoding.UTF8, "application/json");


                var result = web.PostAsync(SettingsStatic.Settings.webhook, content).Result;
            }            
        }
    }
}

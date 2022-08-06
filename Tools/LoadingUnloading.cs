using BalenaNebraUpdater.Tracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalenaNebraUpdater.Tools
{
    public class LoadingUnloading
    {
        private readonly string config = Directory.GetCurrentDirectory()+"\\conf.json";

        public void Save()
        {
            try {
                if (File.Exists(config)) File.Delete(config);
                using (var sw = new StreamWriter(config))
                {
                    Settings tmpParse = new Settings();
                    tmpParse = SettingsStatic.Settings;
                    string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(tmpParse.ApiKey));
                    tmpParse.ApiKey = encoded;
                    encoded = null;
                    sw.WriteLine(JsonConvert.SerializeObject(tmpParse));
                }
            } catch(Exception ex) { Logger.SaveEntry($"Failed Saving Config | {ex.Message}",Objs.Enums.ErrorLevel.Fatal); }        
        }
        public bool Loaded()
        {
            if (File.Exists(config))
            {
                try {
                    var raw = File.ReadAllText(config);
                    var set = JsonConvert.DeserializeObject<Settings>(raw);
                    set.ApiKey = Encoding.UTF8.GetString(Convert.FromBase64String(set.ApiKey));
                    SettingsStatic.Settings = set;
                    return true;
                } catch(Exception ex) { Logger.SaveEntry($"Failed Saving Config | {ex.Message}", Objs.Enums.ErrorLevel.Fatal);  return false; }
            }
            return false;
        }
    }
}

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

        public void Save() { 
        if(File.Exists(config)) File.Delete(config);
            using (var sw = new StreamWriter(config)) {
                sw.WriteLine(JsonConvert.SerializeObject(SettingsStatic.Settings));
            }
        }
        public bool Loaded()
        {
            if (File.Exists(config))
            {
                try {
                    var raw = File.ReadAllText(config);
                    var set = JsonConvert.DeserializeObject<Settings>(raw);
                    SettingsStatic.Settings = set;
                    return true;
                } catch { return false; }                
            }
            return false;
        }
    }
}

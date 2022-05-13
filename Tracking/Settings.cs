using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalenaNebraUpdater.Tracking
{
    public class Settings
    {
        public string? webhook { get; set; }
        public string? CurrentCommit { get; set; }
        public string? FleetName { get; set; }
        public string? BalenaPath { get; set; }

    }
    public class Setting {
        public Settings settings = new Settings();
    }
}

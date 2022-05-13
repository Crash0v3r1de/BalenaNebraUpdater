using BalenaNebraUpdater.Tracking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalenaNebraUpdater.Core
{
    public class Balena
    {
        private readonly string _gitLoc = Environment.CurrentDirectory + "\\helium-rak";

        public bool BalenaLogin() {
            ProcessStartInfo gitInfo = new ProcessStartInfo();
            gitInfo.CreateNoWindow = true;
            gitInfo.RedirectStandardError = true;
            gitInfo.RedirectStandardOutput = true;
            gitInfo.FileName = SettingsStatic.Settings.BalenaPath + @"\balena.cmd";
            Process gitProcess = new Process();


            gitInfo.Arguments = "login --web";
            gitInfo.WorkingDirectory = _gitLoc;

            gitProcess.StartInfo = gitInfo;
            gitProcess.Start();

            string stderr_str = gitProcess.StandardError.ReadToEnd();
            string stdout_str = gitProcess.StandardOutput.ReadToEnd();

            gitProcess.WaitForExit();
            gitProcess.Close();

            if (!String.IsNullOrWhiteSpace(stderr_str)) return false;

            return true;
        }
        public bool FleetPush() {
            ProcessStartInfo gitInfo = new ProcessStartInfo();
            gitInfo.CreateNoWindow = true;
            gitInfo.RedirectStandardError = true;
            gitInfo.RedirectStandardOutput = true;
            gitInfo.FileName = SettingsStatic.Settings.BalenaPath + @"\balena.cmd";
            Process gitProcess = new Process();


            gitInfo.Arguments = $"push {SettingsStatic.Settings.FleetName} -d";
            gitInfo.WorkingDirectory = _gitLoc;

            gitProcess.StartInfo = gitInfo;
            gitProcess.Start();

            string stderr_str = gitProcess.StandardError.ReadToEnd();
            string stdout_str = gitProcess.StandardOutput.ReadToEnd();

            gitProcess.WaitForExit();
            gitProcess.Close();

            if (!String.IsNullOrWhiteSpace(stderr_str)) return false;

            return true;
        }
    }
}

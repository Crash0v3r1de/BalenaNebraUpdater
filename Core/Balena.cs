using BalenaNebraUpdater.Tracking;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using BalenaNebraUpdater.Objs;
using BalenaNebraUpdater.Tools;

namespace BalenaNebraUpdater.Core
{
    public class Balena
    {
        private readonly string _gitLoc = Environment.CurrentDirectory + "\\helium-rak";
        private string ApiKey { get; set; }

        public bool NeedsAuth() {
            ProcessStartInfo gitInfo = new ProcessStartInfo();
            gitInfo.CreateNoWindow = true;
            gitInfo.RedirectStandardError = true;
            gitInfo.RedirectStandardOutput = true;
            gitInfo.FileName = SettingsStatic.Settings.BalenaPath + @"\balena.cmd";
            Process gitProcess = new Process();


            gitInfo.Arguments = "fleets";
            gitInfo.WorkingDirectory = _gitLoc;

            gitProcess.StartInfo = gitInfo;
            gitProcess.Start();

            string stderr_str = gitProcess.StandardError.ReadToEnd();
            string stdout_str = gitProcess.StandardOutput.ReadToEnd();

            gitProcess.WaitForExit();
            gitProcess.Close();

            if (stderr_str.Contains("Login required")) return true;

            return false;
        }
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

        public string ApiPushRepoUpdate() {
            try
            {
                var httpClientHandler = new HttpClientHandler();
                if (StaticDebugger.CurrentlyDebugging) {
                    var proxy = new WebProxy
                    {
                        Address = new Uri("http://127.0.0.1:8888"),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = true,
                        Credentials = new NetworkCredential(userName: "", password: "")
                    };
                    httpClientHandler.Proxy = proxy;
                }
                var web = new HttpClient(httpClientHandler);
                var content = new StringContent("{\"shouldFlatten\":true,\"url\":\"https://github.com/NebraLtd/helium-rak/archive/master.tar.gz\"}", Encoding.UTF8, "application/json"); // statically set URL string, can code to grab this at some point if it ever changes
                web.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SettingsStatic.Settings.ApiKey);
                var result = web.PostAsync($"https://builder.balena-cloud.com/v3/buildFromUrl?headless=true&owner={SettingsStatic.Settings.OrgName}&app={SettingsStatic.Settings.FleetName}", content).Result;
                return result.Content.ReadAsStringAsync().Result;
            } catch(Exception ex) { Logger.SaveEntry($"APIPUSHREPOUPDATE() | {ex.Message}",Objs.Enums.ErrorLevel.Fatal); }


            return null;
        }
        public string GetOrgName(string api) {
            ApiKey = api;
            try
            {
                var httpClientHandler = new HttpClientHandler();
                if (StaticDebugger.CurrentlyDebugging)
                {
                    var proxy = new WebProxy
                    {
                        Address = new Uri("http://127.0.0.1:8888"),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = true,
                        Credentials = new NetworkCredential(userName: "", password: "")
                    };
                    httpClientHandler.Proxy = proxy;
                }
                var web = new HttpClient(httpClientHandler);
                web.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", api);
                var result = web.GetAsync("https://api.balena-cloud.com/v6/organization").Result;
                var tmp = result.Content.ReadAsStringAsync().Result; 

                return Regex.Match(tmp, "handle\":\"(.*?)\"").Groups[1].Value; // Regex > Deserialize
            }
            catch (Exception ex) { Logger.SaveEntry($"GETORGNAME() | {ex.Message}", Objs.Enums.ErrorLevel.Fatal); }


            return null;
        }
        public string GetCurrentCommit()
        {
            try
            {
                var httpClientHandler = new HttpClientHandler();
                if (StaticDebugger.CurrentlyDebugging)
                {
                    var proxy = new WebProxy
                    {
                        Address = new Uri("http://127.0.0.1:8888"),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = true,
                        Credentials = new NetworkCredential(userName: "", password: "")
                    };
                    httpClientHandler.Proxy = proxy;
                }
                var web = new HttpClient(httpClientHandler);
                web.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("BalenaUpdaterTool","1.0"));
                var result = web.GetAsync("https://api.github.com/repos/NebraLtd/helium-rak/commits").Result;
                var commits = JsonConvert.DeserializeObject<List<GithubCommits>>(result.Content.ReadAsStringAsync().Result);

                return commits[0].sha;
            }
            catch (Exception ex) { Logger.SaveEntry($"GETCURRENTCOMMIT() | {ex.Message}", Objs.Enums.ErrorLevel.Fatal); }


            return null;
        }
        public List<string> GetFleets(string api)
        {
            ApiKey = api;
            try
            {
                List<string> ours = new List<string>();
                var httpClientHandler = new HttpClientHandler();
                if (StaticDebugger.CurrentlyDebugging)
                {
                    var proxy = new WebProxy
                    {
                        Address = new Uri("http://127.0.0.1:8888"),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = true,
                        Credentials = new NetworkCredential(userName: "", password: "")
                    };
                    httpClientHandler.Proxy = proxy;
                }
                var web = new HttpClient(httpClientHandler);
                web.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", api);
                var result = web.GetAsync("https://api.balena-cloud.com/v6/application").Result;
                var tmp = result.Content.ReadAsStringAsync().Result;

                var found = Regex.Matches(tmp, "slug\":\"(.*?)\"");
                foreach (Match match in found) {
                    if (match.Groups[1].Value.Contains(SettingsStatic.Settings.OrgName)) { 
                    // we own this fleet
                    ours.Add(match.Groups[1].Value);
                    }
                }



                return ours;
            }
            catch (Exception ex) { Logger.SaveEntry($"GETFLEETS() | {ex.Message}", Objs.Enums.ErrorLevel.Fatal); }


            return null;
        }
    }
}

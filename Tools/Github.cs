using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BalenaNebraUpdater.Tracking;
using LibGit2Sharp;

namespace BalenaNebraUpdater.Tools
{
    public class Github
    {
        private readonly string _gitLoc = Environment.CurrentDirectory + "\\helium-rak";

        public bool NeedsCloned() {
            if (Directory.Exists(_gitLoc)) return false;
            return true;
        }
        public bool CloneRepo() {
            try {
                Repository.Clone("https://github.com/NebraLtd/helium-rak.git", _gitLoc);
            } catch { return false; }
            return true;
        }
        public bool PullRepo() {
            try
            {
                using (var repo = new Repository(_gitLoc)) {
                    var sig = new Signature(new Identity("no_one", "none@gmail.com"), DateTimeOffset.Now);


                    LibGit2Sharp.PullOptions options = new LibGit2Sharp.PullOptions();
                    options.FetchOptions = new FetchOptions();
                    Commands.Pull(repo,sig,options);
                }
            }
            catch { return false; }
            return true;
        }
        private string CurrentCommitHash() {
            string current = "";
            using (var repo = new Repository(_gitLoc))
            {
                foreach (Branch b in repo.Branches.Where(b => !b.IsRemote))
                {
                    if (b.FriendlyName.Contains("master")) {
                        var logs = b.TrackedBranch.Commits;
                        foreach (var item in logs)
                        {
                            var tmp = item.Id;
                            current = tmp.Sha;
                                break; // we just want the newest (first listed)
                        }
                    }
                }
            }
            return current;
        }
        public bool NeedsPulled() {
            if (CurrentCommitHash() != SettingsStatic.Settings.CurrentCommit) return true;
            return false;
        }
    }
}

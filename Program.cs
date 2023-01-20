using BalenaNebraUpdater.Core;
using BalenaNebraUpdater.Tools;
using BalenaNebraUpdater.Tracking;
using System.Diagnostics;

Console.CancelKeyPress += delegate {
    new LoadingUnloading().Save();
    Environment.Exit(0); // User canceled program
};
Console.WriteLine($"{DateTime.Now} | Program started...");
LoadingUnloading ld = new LoadingUnloading();
ConsoleHelp con = new ConsoleHelp();
Balena belInitial = new Balena();

if (args.Length != 0) { 
if(args[0] == "true") StaticDebugger.CurrentlyDebugging = true;
}
// When console menu is made a menu for picking the fleet name from API will also be added so this is commented for that process
//bel.GetFleets("key");


if (!ld.Loaded()) { // No settings - prompt for initial config
    SettingsStatic.Settings.ApiKey = con.BalenaApiKey();
    SettingsStatic.Settings.OrgName = belInitial.GetOrgName(SettingsStatic.Settings.ApiKey);
    SettingsStatic.Settings.webhook = con.PromptWebhook();
    // Bellow is commented out since balenacli option is not coded as an option yet
    //SettingsStatic.Settings.BalenaPath = con.BalenaPath();
    SettingsStatic.Settings.FleetName = con.FleetName();
    ld.Save();
}

// Leftover debug code
//BalenaStatus.NeedsAuth = true;
//if (bel.NeedsAuth()) bel.BalenaLogin(); BalenaStatus.NeedsAuth = false;

while (true) {
    Github git = new Github();
    Balena bel = new Balena();


    try {
        bool updated = false;
        string output;
        string currentCommit = bel.GetCurrentCommit();
        if (SettingsStatic.Settings.CurrentCommit != null & SettingsStatic.Settings.CurrentCommit != currentCommit)
        {
            // new commit found
            output = bel.ApiPushRepoUpdate();
            if (output.Contains("started\":true")) {
                Console.WriteLine($"{DateTime.Now} | Build image has been updated to the newest repo commit");
                SettingsStatic.Settings.CurrentCommit = currentCommit;
                try { ld.Save(); } catch { Console.WriteLine($"{DateTime.Now} | Failed to save settings after commit update - please report this as an issue on Github"); }
                Discord.SendWebhook();
                ReloadMyself();
            }
            else Console.WriteLine($"{DateTime.Now} | Commit update push failed - please report this as an issue on Github"); ReloadMyself();
                                                                                    // We'll reload ourself on failure to see if maybe the token just went stale anyway
        }
        else
        {            
            if (SettingsStatic.Settings.CurrentCommit == null)
            {
                output = bel.ApiPushRepoUpdate();
                if (output.Contains("started\":true")) {
                    Console.WriteLine($"{DateTime.Now} | Initial build started, commit is now current");
                    SettingsStatic.Settings.CurrentCommit = currentCommit;
                    try { ld.Save(); } catch { Console.WriteLine($"{DateTime.Now} | Failed to save settings after initial commit update - please report this as an issue on Github"); }
                    Discord.SendWebhook();
                    ReloadMyself();
                }
                else Console.WriteLine($"{DateTime.Now} | Initial build failed - please report this as an issue on Github"); ReloadMyself();
            }                                                                       // We'll reload ourself on failure to see if maybe the token just went stale anyway
            else Console.WriteLine($"{DateTime.Now} | Update not needed");

        }
    } catch(Exception ex) { Console.WriteLine("FATAL ERROR! Please report an issue to github | Failed logic loop"); Logger.SaveEntry($"Logic loop failed | {ex.Message}",BalenaNebraUpdater.Objs.Enums.ErrorLevel.Fatal); ReloadMyself(); }
                                                                                    // We'll reload ourself on failure to see if maybe the token just went stale anyway

    // Add console menu options to use the balenacli option, this code would then be used. Api is just better for easability
    //if (git.NeedsCloned())
    //{
    //    git.CloneRepo();
    //}
    //if (git.NeedsPulled())
    //{
    //    // Make commit comparison method
    //    git.PullRepo(); // Update
    //    if (BalenaStatus.NeedsAuth) bel.BalenaLogin(); BalenaStatus.NeedsAuth = false; // Prompt for login if initial run
    //    bel.FleetPush(); // Push update to balena for building
    //    if (!String.IsNullOrEmpty(SettingsStatic.Settings.webhook)) Discord.SendWebhook();
    //    Console.WriteLine($"{DateTime.Now} | Fleet updated!");
    //    updated = true;
    //}
    //if (!updated) { Console.WriteLine($"{DateTime.Now} | Update not needed"); }
    Thread.Sleep(3600000); // hard coded hour wait for now
}

static void ReloadMyself()
{
    // This is a quick workaround for the token issues with Balena push after an initial fleet update, restart the app so the token is valid next update push
    // Whenever I get time (if I get time) to get another capture of multi update pushes to balena I'll reverse it and see what changes for the second push and update this proj
    try {
        Process.Start(Environment.ProcessPath);
        Environment.Exit(4); // We'll use exit code 4 to mark that this is basically restarting a new process
    }
    catch(Exception ex) { Console.WriteLine("Error occured trying to start ourself after image push"); Console.WriteLine(ex.Message); }
}
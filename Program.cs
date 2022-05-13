// See https://aka.ms/new-console-template for more information
using BalenaNebraUpdater.Core;
using BalenaNebraUpdater.Tools;
using BalenaNebraUpdater.Tracking;
Console.CancelKeyPress += delegate {
    new LoadingUnloading().Save();
    Environment.Exit(0); // User canceled program
};
ConsoleHelp con = new ConsoleHelp();
Github git = new Github();
Balena bel = new Balena();

if (!new LoadingUnloading().Loaded()) { // No settings - prompt for initial config
    SettingsStatic.Settings.webhook = con.PromptWebhook();
    SettingsStatic.Settings.BalenaPath = con.BalenaPath();
    SettingsStatic.Settings.FleetName = con.FleetName();
}

while (true) {
    // Version checking with github logic - may be organized into it's own class down the road
    if (git.NeedsCloned())
    {
        if (git.NeedsPulled())
        {
            git.PullRepo(); // Update
            if (BalenaStatus.NeedsAuth) bel.BalenaLogin(); // Prompt for login if initial run
            bel.FleetPush(); // Push update to balena for building
            if (!String.IsNullOrEmpty(SettingsStatic.Settings.webhook)) Discord.SendWebhook();
            Console.WriteLine($"{DateTime.Now} | Fleet updated!");
        }
        Console.WriteLine($"{DateTime.Now} | Update not needed");
    }
    Thread.Sleep(60000);
}


Console.WriteLine($"Current Commit: {SettingsStatic.Settings.CurrentCommit} | Repo Current Commit: ");

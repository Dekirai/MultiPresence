#nullable disable
using MultiPresence.Runtime;

namespace MultiPresence.Presence;

public static class GFR
{
    public static Task DoAction() => SteamPresenceLauncher.LaunchAsync("Gunfire Reborn", 1217060);
}

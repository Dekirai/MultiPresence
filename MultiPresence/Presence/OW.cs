#nullable disable
using MultiPresence.Runtime;

namespace MultiPresence.Presence;

public static class OW
{
    public static Task DoAction() => SteamPresenceLauncher.LaunchAsync("Overwatch", 2357570);
}

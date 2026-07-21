#nullable disable
using MultiPresence.Runtime;

namespace MultiPresence.Presence;

public static class COD
{
    public static Task DoAction() => SteamPresenceLauncher.LaunchAsync("cod", 1938090);
}

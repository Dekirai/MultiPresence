#nullable disable
using MultiPresence.Runtime;

namespace MultiPresence.Presence;

public static class HK
{
    public static Task DoAction() => SteamPresenceLauncher.LaunchAsync("Hello Kitty", 2495100);
}

#nullable disable
using MultiPresence.Runtime;

namespace MultiPresence.Presence;

public static class D4
{
    public static Task DoAction() => SteamPresenceLauncher.LaunchAsync("Diablo IV", 2344520);
}

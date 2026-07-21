#nullable disable
using MultiPresence.Runtime;

namespace MultiPresence.Presence;

public static class HL
{
    public static Task DoAction() => SteamPresenceLauncher.LaunchAsync("HogwartsLegacy", 990080);
}

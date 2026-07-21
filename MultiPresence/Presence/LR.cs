#nullable disable
using MultiPresence.Runtime;

namespace MultiPresence.Presence;

public static class LR
{
    public static Task DoAction() => SteamPresenceLauncher.LaunchAsync("Labyrinthine", 1302240);
}

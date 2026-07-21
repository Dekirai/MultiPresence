#nullable disable
using MultiPresence.Runtime;

namespace MultiPresence.Presence;

public static class TTS
{
    public static Task DoAction() => SteamPresenceLauncher.LaunchAsync("TemtemSwarm", 2510960);
}

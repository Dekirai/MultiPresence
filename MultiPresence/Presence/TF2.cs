#nullable disable
using MultiPresence.Runtime;

namespace MultiPresence.Presence;

public static class TF2
{
    public static Task DoAction() => SteamPresenceLauncher.LaunchAsync("tf_win64", 440);
}

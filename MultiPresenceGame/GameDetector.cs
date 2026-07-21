using System.Diagnostics;

namespace MultiPresenceGame;

internal static class GameDetector
{
    private static readonly IReadOnlyList<(string ProcessName, string GameName)> Games =
    [
        ("cod", "Call of Duty®"),
        ("Diablo IV", "Diablo IV"),
        ("Gunfire Reborn", "Gunfire Reborn"),
        ("Hello Kitty", "Hello Kitty Island Adventure"),
        ("HogwartsLegacy", "Hogwarts Legacy"),
        ("Labyrinthine", "Labyrinthine"),
        ("Overwatch", "Overwatch"),
        ("tf_win64", "Team Fortress 2"),
        ("TemtemSwarm", "Temtem: Swarm")
    ];

    public static string GetGame()
    {
        Process[] processes = [];
        try
        {
            processes = Process.GetProcesses();
            var names = processes.Select(SafeName).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var (processName, gameName) in Games)
            {
                if (names.Contains(processName))
                {
                    return gameName;
                }
            }

            return string.Empty;
        }
        finally
        {
            foreach (var process in processes)
            {
                process.Dispose();
            }
        }
    }

    private static string SafeName(Process process)
    {
        try
        {
            return process.ProcessName;
        }
        catch
        {
            return string.Empty;
        }
    }
}

using System.Diagnostics;

namespace MultiPresenceGame
{
    public static class GameDetector
    {
        // Mapping process names to game titles
        // {"processname", "Game Title"}

        private static readonly Dictionary<string, string> GameMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "cod", "Call of Duty®" },
            { "Gunfire Reborn", "Gunfire Reborn" },
            { "Hello Kitty", "Hello Kitty Island Adventure" },
            { "HogwartsLegacy", "Hogwarts Legacy" },
            { "Labyrinthine", "Labyrinthine" },
            { "Overwatch", "Overwatch 2" },
            { "tf_win64", "Team Fortress 2" },
            { "SB-Win64-Shipping", "Stellar Blade" },
            { "TemtemSwarm", "Temtem: Swarm" }
        };

        public static string GetGame()
        {
            var processes = Process
                            .GetProcesses()
                            .DistinctBy(p => p.ProcessName, StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(
                            p => p.ProcessName,
                            StringComparer.OrdinalIgnoreCase
            );

            return string.Empty;
        }
    }
}

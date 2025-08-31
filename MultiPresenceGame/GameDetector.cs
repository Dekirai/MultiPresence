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

            foreach (var kvp in GameMap)
            {
                if (processes.ContainsKey(kvp.Key))
                {
                    // Special-case 'game' for Mega Man 11, Persona 5 Strikers, etc.
                    return kvp.Key == "game"
                        ? DetectGameTitle(processes["game"]) ?? kvp.Value
                        : kvp.Value;
                }
            }

            return string.Empty;
        }

        private static string? DetectGameTitle(Process gameProcess)
        {
            var title = gameProcess.MainWindowTitle;
            return title.Contains("MEGAMAN11") ? "Mega Man 11"
                 : title.Contains("Persona 5 Strikers") ? "Persona 5 Strikers"
                 : null;
        }
    }
}

using System.Globalization;

namespace MultiPresence.Models.PHAS
{
    public static class GameText
    {
        private static readonly Dictionary<(int LevelType, bool Restricted), string> Maps = new Dictionary<(int LevelType, bool Restricted), string>

        {
            [(5, false)] = "10 Ridgeview Court",
            [(10, false)] = "13 Willow Street",
            [(4, false)] = "42 Edgefield Road",
            [(2, false)] = "6 Tanglewood Drive",
            [(7, false)] = "Bleasdale Farmhouse",
            [(6, true)] = "Brownstone High School Restricted",
            [(6, false)] = "Brownstone High School",
            [(12, false)] = "Camp Woodwind",
            [(8, false)] = "Grafton Farmhouse",
            [(11, false)] = "Maple Lodge Campsite",
            [(14, false)] = "Nell's Diner",
            [(13, true)] = "Point Hope Restricted",
            [(13, false)] = "Point Hope",
            [(9, true)] = "Prison Restricted",
            [(9, false)] = "Prison",
            [(0, false)] = "Random",
            [(3, true)] = "Sunny Meadows Restricted",
            [(3, false)] = "Sunny Meadows",
            [(15, false)] = "Test_Map",
            [(1, false)] = "Training",
        };
        private static readonly Dictionary<(int LevelType, bool Restricted, int SeasonalEvent), string> EventMaps = new Dictionary<(int LevelType, bool Restricted, int SeasonalEvent), string>

        {
            [(7, false, 5)] = "Elderwood Palace Lodge",
            [(12, false, 5)] = "Coffee World",
            [(8, false, 5)] = "Cauldron Lake",
            [(14, false, 5)] = "Oh Deer Diner",
        };
        private static readonly Dictionary<int, string> Difficulties = new Dictionary<int, string>
        {
            { 0, "Amateur" },
            { 1, "Intermediate" },
            { 2, "Professional" },
            { 3, "Nightmare" },
            { 4, "Insanity" },
            { 5, "Challenge Mode" },
            { 6, "Custom" },
            { 7, "Custom" },
            { 8, "Custom" },
            { 9, "Apocalypse" },
            { 10, "Apocalypse" },
            { 11, "Apocalypse" }
        };
        private static readonly Dictionary<string, string> DifficultyKeys = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Contract_Amateur", "Amateur" },
            { "Contract_Intermediate", "Intermediate" },
            { "Contract_Professional", "Professional" },
            { "Contract_Nightmare", "Nightmare" },
            { "Contract_Insanity", "Insanity" },
            { "Contract_Apocalypse", "Apocalypse" },
            { "Contract_Challenge", "Challenge Mode" },
            { "Contract_Custom", "Custom" }
        };
        public static string GetDifficulty(int id, string nameKey = "")
        {
            if (!string.IsNullOrEmpty(nameKey) && DifficultyKeys.TryGetValue(nameKey, out var localized))
            {
                return localized;
            }

            return Difficulties.TryGetValue(id, out var name) ? name : id.ToString(CultureInfo.InvariantCulture);
        }

        public static string GetMap(int levelType, bool restricted = false, int seasonalEvent = 0)
        {
            if (EventMaps.TryGetValue((levelType, restricted, seasonalEvent), out var eventName))
            {
                return eventName;
            }

            return Maps.TryGetValue((levelType, restricted), out var name) ? name : levelType.ToString(CultureInfo.InvariantCulture);
        }

        public static string GetRoom(string nativeName)
        {
            if (string.IsNullOrWhiteSpace(nativeName) || nativeName.Any(char.IsControl))
            {
                return "";
            }

            return nativeName.Trim();
        }
    }
}

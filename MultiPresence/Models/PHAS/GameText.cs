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
        private static readonly Dictionary<string, string> Scenes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Ridgeview_Road_House", "10 Ridgeview Court" },
            { "Willow_Street_House", "13 Willow Street" },
            { "Edgefield_Street_House", "42 Edgefield Road" },
            { "Tanglewood_Street_House", "6 Tanglewood Drive" },
            { "Bleasdale_Farmhouse", "Bleasdale Farmhouse" },
            { "Bleasdale_Farmhouse Alan Wake", "Elderwood Palace Lodge" },
            { "Brownstone_High_School", "Brownstone High School" },
            { "Camp_Woodwind", "Camp Woodwind" },
            { "Camp_Woodwind Alan Wake", "Coffee World" },
            { "Grafton_Farmhouse", "Grafton Farmhouse" },
            { "Grafton_Farmhouse Alan Wake", "Cauldron Lake" },
            { "Maple_Lodge_Campsite", "Maple Lodge Campsite" },
            { "Nells_Diner", "Nell's Diner" },
            { "Nells_Diner Alan Wake", "Oh Deer Diner" },
            { "Point_Hope", "Point Hope" },
            { "Prison", "Prison" },
            { "Sunny_Meadows", "Sunny Meadows" },
            { "Test_Level", "Test_Map" },
            { "Training", "Training" },
            { "Edgefield_Street_House Halloween", "42 Edgefield Road" },
            { "Grafton_Farmhouse Halloween", "Grafton Farmhouse" },
            { "Point_Hope Halloween", "Point Hope" },
            { "Willow_Street_House Halloween", "13 Willow Street" },
            { "Bleasdale_Farmhouse Holiday", "Bleasdale Farmhouse" },
            { "Maple_Lodge_Campsite Holiday", "Maple Lodge Campsite" },
            { "Point_Hope Holiday", "Point Hope" },
            { "Bleasdale_Farmhouse Easter", "Bleasdale Farmhouse" },
            { "Tanglewood_Street_House Easter", "6 Tanglewood Drive" },
            { "Tanglewood_Street_House Holiday", "6 Tanglewood Drive" },
            { "Tanglewood_Street_House None", "6 Tanglewood Drive" },
            { "Edgefield_Street_House None", "42 Edgefield Road" },
            { "Edgefield_Street_House Easter", "42 Edgefield Road" },
            { "Tanglewood_Street_House Halloween", "6 Tanglewood Drive" },
            { "Bleasdale_Farmhouse Halloween", "Bleasdale Farmhouse" },
            { "Camp_Woodwind Halloween", "Camp Woodwind" },
            { "Willow_Street_House None", "13 Willow Street" },
            { "Bleasdale_Farmhouse None", "Bleasdale Farmhouse" },
            { "Camp_Woodwind None", "Camp Woodwind" },
            { "Grafton_Farmhouse None", "Grafton Farmhouse" },
            { "Point_Hope None", "Point Hope" },
            { "Nells_Diner None", "Nell's Diner" },
            { "Willow_Street_House Holiday", "13 Willow Street" },
            { "Ridgeview_Road_House Holiday", "10 Ridgeview Court" },
            { "Nells_Diner Holiday", "Nell's Diner" },
            { "Grafton_Farmhouse Holiday", "Grafton Farmhouse" },
            { "Edgefield_Street_House Holiday", "42 Edgefield Road" },
            { "Camp_Woodwind Holiday", "Camp Woodwind" },
            { "Maple_Lodge_Campsite Easter", "Maple Lodge Campsite" },
            { "Maple_Lodge_Campsite None", "Maple Lodge Campsite" },
            { "Ridgeview_Road_House None", "10 Ridgeview Court" },
            { "Grafton_Farmhouse Easter", "Grafton Farmhouse" },
            { "Camp_Woodwind Easter", "Camp Woodwind" },
            { "Ridgeview_Road_House Easter", "10 Ridgeview Court" },
            { "Point_Hope Easter", "Point Hope" },
            { "Nells_Diner Easter", "Nell's Diner" },
            { "Willow_Street_House Easter", "13 Willow Street" },
            { "Test_Level None", "Test_Map" },
            { "Nells_Diner Halloween", "Nell's Diner" }
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

        public static string GetMapByScene(string scene)
        {
            if (string.IsNullOrWhiteSpace(scene))
            {
                return "";
            }

            var key = scene.Trim().Replace('\\', '/');
            key = key[(key.LastIndexOf('/') + 1)..];
            if (key.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
            {
                key = key[..^6];
            }

            return Scenes.TryGetValue(key, out var name) ? name : key;
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

namespace MultiPresence.Models.SR2
{
    internal static class GameText
    {
        public static string GetLocation(int region)
        {
            return Locations.TryGetValue(region, out var location) ? location : "";
        }

        public static string GetRealm(int plane)
        {
            return plane switch
            {
                1 => "Material Realm",
                2 => "Spectral Realm",
                _ => "",
            };
        }

        public static string GetReaver(int type)
        {
            return Reavers.TryGetValue(type, out var reaver) ? reaver : "";
        }

        private static readonly Dictionary<int, string> Locations = new()
        {
            { 1, "Time Streaming Device" },
            { 2, "Time Streaming Device" },
            { 3, "Sarafan Stronghold" },
            { 4, "Janos Audron's Retreat" },
            { 5, "Light Forge" },
            { 6, "Dark Forge" },
            { 7, "Air Forge" },
            { 8, "Fire Forge" },
            { 9, "Southern Lake" },
            { 10, "The Pillars" },
            { 11, "Subterranean Ruins" },
            { 12, "Swamp" },
            { 13, "Uschtenheim" },
        };

        private static readonly Dictionary<int, string> Reavers = new()
        {
            { 1, "Spectral Reaver" },
            { 2, "Material Reaver" },
            { 3, "Dark Reaver" },
            { 4, "Light Reaver" },
            { 5, "Air Reaver" },
            { 6, "Fire Reaver" },
            { 7, "Water Reaver" },
            { 8, "Earth Reaver" },
            { 9, "Spirit Reaver" },
        };
    }
}

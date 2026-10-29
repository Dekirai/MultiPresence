namespace MultiPresence.Models.SR1
{
    internal static class GameText
    {
        public static string GetLocation(string area)
        {
            var digit = 0;
            while (digit < area.Length && (area[digit] < '0' || area[digit] > '9'))
            {
                digit++;
            }

            if (digit < area.Length && int.TryParse(area[digit..], out var number) && Locations.TryGetValue(area[..digit], out var ranges))
            {
                foreach (var range in ranges)
                {
                    if (number >= range.Start && number <= range.End)
                    {
                        return range.Name;
                    }
                }
            }

            return area;
        }

        public static string GetObjective(int index)
        {
            return index >= 0 && index < Objectives.Length ? Objectives[index] : "";
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

        private static readonly Dictionary<string, (int Start, int End, string Name)[]> Locations = new Dictionary<string, (int Start, int End, string Name)[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "under", new (int Start, int End, string Name)[] { (1, 4, "Underworld") } },
            { "train", new (int Start, int End, string Name)[] { (1, 6, "Underworld"), (7, 8, "Underworld Exit"), (10, 12, "Underworld Exit"), (9, 9, "Sanctuary") } },
            { "pillars", new (int Start, int End, string Name)[] { (1, 6, "Sanctuary"), (8, 10, "Sanctuary") } },
            { "clfsubb", new (int Start, int End, string Name)[] { (1, 3, "Lake of the Dead") } },
            { "cliff", new (int Start, int End, string Name)[] { (1, 1, "Lake of the Dead") } },
            { "clfpil", new (int Start, int End, string Name)[] { (1, 4, "Lake of the Dead") } },
            { "huba", new (int Start, int End, string Name)[] { (1, 20, "Raziel's Stronghold") } },
            { "fire", new (int Start, int End, string Name)[] { (1, 9, "Fire Glyph") } },
            { "skinnr", new (int Start, int End, string Name)[] { (1, 7, "Necropolis"), (9, 18, "Necropolis") } },
            { "out", new (int Start, int End, string Name)[] { (1, 11, "Necropolis"), (13, 16, "Force Glyph") } },
            { "stone", new (int Start, int End, string Name)[] { (1, 9, "Stone Glyph"), (13, 15, "Stone Glyph"), (10, 11, "Stone Glyph") } },
            { "cathy", new (int Start, int End, string Name)[] { (1, 13, "Silenced Cathedral"), (15, 19, "Silenced Cathedral"), (21, 22, "Silenced Cathedral"), (24, 25, "Silenced Cathedral"), (27, 28, "Silenced Cathedral"), (30, 31, "Silenced Cathedral"), (33, 33, "Silenced Cathedral"), (36, 40, "Silenced Cathedral"), (42, 42, "Silenced Cathedral"), (45, 55, "Silenced Cathedral"), (57, 62, "Silenced Cathedral"), (64, 65, "Silenced Cathedral"), (67, 76, "Silenced Cathedral") } },
            { "soundg", new (int Start, int End, string Name)[] { (3, 6, "Sound Glyph") } },
            { "add", new (int Start, int End, string Name)[] { (1, 8, "Sarafan Tomb") } },
            { "bonus", new (int Start, int End, string Name)[] { (1, 1, "Sarafan Tomb") } },
            { "boss", new (int Start, int End, string Name)[] { (2, 2, "Sarafan Tomb") } },
            { "chapel", new (int Start, int End, string Name)[] { (4, 4, "Sarafan Tomb") } },
            { "pillar", new (int Start, int End, string Name)[] { (3, 3, "Sarafan Tomb") } },
            { "tomnb", new (int Start, int End, string Name)[] { (1, 1, "Sarafan Tomb") } },
            { "tompil", new (int Start, int End, string Name)[] { (1, 5, "Sarafan Tomb") } },
            { "conect", new (int Start, int End, string Name)[] { (1, 1, "Sarafan Tomb") } },
            { "conecta", new (int Start, int End, string Name)[] { (1, 1, "Sarafan Tomb") } },
            { "conectb", new (int Start, int End, string Name)[] { (1, 1, "Sarafan Tomb") } },
            { "conectc", new (int Start, int End, string Name)[] { (1, 1, "Sarafan Tomb") } },
            { "conectd", new (int Start, int End, string Name)[] { (1, 1, "Sarafan Tomb") } },
            { "conecte", new (int Start, int End, string Name)[] { (1, 1, "Sarafan Tomb") } },
            { "conectg", new (int Start, int End, string Name)[] { (1, 5, "Drowned Abbey") } },
            { "aluka", new (int Start, int End, string Name)[] { (1, 12, "Drowned Abbey"), (14, 15, "Drowned Abbey"), (19, 25, "Drowned Abbey"), (27, 34, "Drowned Abbey"), (36, 41, "Drowned Abbey"), (43, 48, "Drowned Abbey") } },
            { "city", new (int Start, int End, string Name)[] { (1, 9, "Human Citadel"), (13, 15, "Human Citadel"), (19, 21, "Human Citadel") } },
            { "cityout", new (int Start, int End, string Name)[] { (1, 5, "Human Citadel") } },
            { "mrlock", new (int Start, int End, string Name)[] { (7, 8, "Human Citadel"), (1, 2, "Oracle's Cave"), (14, 14, "Oracle's Cave") } },
            { "tower", new (int Start, int End, string Name)[] { (1, 6, "Human Citadel"), (7, 12, "Water Glyph") } },
            { "fill", new (int Start, int End, string Name)[] { (1, 3, "Sunlight Glyph") } },
            { "filtuna", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "filtunab", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "gastuna", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "intvaly", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "litein", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "litetun", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "litrnda", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "litrndb", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "pisaira", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "pisairb", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "piston", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "pistunl", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "sunrm", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "suntnl", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "suntnlb", new (int Start, int End, string Name)[] { (1, 1, "Sunlight Glyph") } },
            { "hubb", new (int Start, int End, string Name)[] { (1, 4, "Ruined City") } },
            { "nighta", new (int Start, int End, string Name)[] { (10, 13, "Ruined City") } },
            { "nightb", new (int Start, int End, string Name)[] { (8, 11, "Ruined City") } },
            { "oracle", new (int Start, int End, string Name)[] { (1, 24, "Oracle's Cave") } },
            { "chrono", new (int Start, int End, string Name)[] { (1, 19, "Chronoplast") } },
        };

        private static readonly string[] Objectives = new string[]
        {
            "Escape the Underworld",
            "Visit the Lake of the Dead",
            "Investigate your clan territory",
            "Traverse the Necropolis",
            "Defeat Melchiah",
            "Enter the Sanctuary of the Clans",
            "Search through the Silenced Cathedral",
            "Defeat Zephon",
            "Find the hidden tomb",
            "Defeat the Tomb Guardian",
            "Explore the Drowned Abbey",
            "Defeat Rahab",
            "Discover the Ruined City",
            "Defeat Dumah",
            "Venture into the Oracle's Cave",
            "Delve into the depths of the Chronoplast",
            "Defeat Kain",
        };
    }
}

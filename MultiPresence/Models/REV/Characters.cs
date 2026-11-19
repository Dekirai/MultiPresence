namespace MultiPresence.Models.REV
{
    internal static class Characters
    {
        private static readonly (string Model, string Name)[] Names =
        [
            ("Chris", "Chris Redfield"), ("Jill", "Jill Valentine"), ("Parker", "Parker Luciani"),
            ("Jessica", "Jessica Sherawat"), ("Keith", "Keith Lumley"), ("Quint", "Quint Ketcham"),
            ("OBrien", "Clive R. O'Brian"), ("Reymond", "Raymond Vester"), ("Morgan", "Morgan Lansdale"),
            ("Norman", "Jack Norman"), ("Rachel", "Rachael Foley"), ("Hunk", "HUNK")
        ];

        public static string GetName(string type)
        {
            // Player classes are named like "uPl< Jill >" or "uPl< ParkerLight >".
            if (!type.StartsWith("uPl< ", StringComparison.Ordinal) || !type.EndsWith(" >", StringComparison.Ordinal)) return "";
            string model = type[5..^2];
            foreach (var (prefix, name) in Names)
            {
                if (model.StartsWith(prefix, StringComparison.Ordinal)) return name;
            }
            return "";
        }

        public static string GetName(uint type)
        {
            // Player class ids are 0x8001VVC0: VV selects the costume, C the character in the order above.
            if ((type & 0xFFFF000F) != 0x80010000) return "";
            int character = (int)(type >> 4 & 0xF);
            return character < Names.Length ? Names[character].Name : "";
        }
    }
}

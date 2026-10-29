namespace MultiPresence.Models.FFXVI
{
    public static class Characters
    {
        private static readonly Dictionary<uint, string> Names = new Dictionary<uint, string>
        {
            { 1, "Clive" },
            { 2, "Clive" },
            { 3, "Clive" },
            { 4, "Clive" },
            { 5, "Joshua" },
            { 6, "Clive" },
            { 100, "Torgal" },
            { 111, "Joshua" },
            { 120, "Jill" },
            { 121, "Jill" },
            { 130, "Cid" },
            { 200, "Gav" },
            { 201, "Byron" },
            { 202, "Tyler" },
            { 203, "Wade" },
            { 204, "Lord Commander Murdoch" },
            { 205, "Tiamat" },
            { 206, "Aevis" },
            { 208, "Theodore" },
            { 209, "Wade" },
            { 220, "Shula" },
            { 1401, "Phoenix" },
            { 1402, "Eikon of Fire" },
            { 1801, "Ifrit" },
            { 1802, "Garuda" },
            { 1803, "Garuda" },
            { 2010, "Infernal Eikon" },
            { 2012, "Infernal Eikon" },
            { 2013, "Ifrit" },
            { 2122, "Typhon" },
            { 2123, "Ifrit" },
            { 2124, "Typhon the Transgressor" },
            { 2550, "Titan" },
            { 2551, "Titan Lost" },
            { 2552, "Ifrit" },
            { 2556, "Titan" },
            { 2722, "Ifrit" },
            { 2723, "Phoenix" },
            { 2724, "Ifrit Risen" },
            { 2725, "Bahamut" },
            { 2726, "Bahamut" },
            { 3025, "Odin" },
            { 3201, "Ifrit, Phoenix, & Bahamut" },
            { 3205, "Ultima Risen" },
            { 3511, "Leviathan" },
            { 3512, "Leviathan" },
            { 3514, "Ifrit" },
            { 3515, "Ifrit" }
        };

        public static string GetName(uint id)
        {
            return Names.TryGetValue(id, out var name) ? name : string.Empty;
        }
    }
}

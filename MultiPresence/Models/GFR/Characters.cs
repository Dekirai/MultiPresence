namespace MultiPresence.Models.GFR
{
    public static class Characters
    {
        private static readonly Dictionary<int, string> Names = new Dictionary<int, string>
        {
            { 201, "Ao Bai" },
            { 205, "Crown Prince" },
            { 206, "Qing Yan" },
            { 207, "Lei Luo" },
            { 212, "Tao" },
            { 213, "Qian Sui" },
            { 214, "Xing Zhe" },
            { 215, "Li" },
            { 216, "Zi Xiao" },
            { 217, "Nona" },
            { 218, "Lyn" },
            { 219, "Momo" },
            { 220, "Cang Jue" },
            { 221, "Yoyo" },
        };

        public static string GetCharacter(int character)
        {
            return Names.TryGetValue(character, out var name) ? name : "Unknown Character";
        }
    }
}

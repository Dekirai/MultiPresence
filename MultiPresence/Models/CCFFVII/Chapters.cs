using System.Globalization;

namespace MultiPresence.Models.CCFFVII
{
    public static class Chapters
    {
        public static string GetName(int chapter)
        {
            return chapter == 0 ? "Prologue" : chapter == 11 ? "Epilogue" : chapter >= 1 && chapter <= 10 ? "Chapter " + chapter.ToString(CultureInfo.InvariantCulture) : string.Empty;
        }
    }
}

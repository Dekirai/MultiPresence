namespace MultiPresence.Models.FFVIIR
{
    internal static class CampaignReader
    {
        public static GameSnapshot Read(ulong manager, GameSnapshot snapshot)
        {
            if (manager == 0)
            {
                throw new InvalidDataException("FFVIIR: game data is unavailable.");
            }

            var nativeChapter = Hypervisor.Read<byte>(manager + 0x437F5, true);
            var chapter = Chapters.GetNumber(nativeChapter);
            var intermission = Chapters.IsIntermission(nativeChapter);
            var difficulty = GetDifficulty(manager);
            return snapshot with
            {
                Campaign = chapter > 0 ? intermission ? "INTERmission" : "Main Story" : "Unknown",
                Chapter = chapter,
                ChapterName = chapter > 0 ? Chapters.GetName(chapter, intermission) : "Unknown",
                ChapterId = chapter > 0 ? "Chapt_" + (intermission ? chapter + 20 : chapter).ToString("D4") : "",
                Difficulty = difficulty
            };
        }

        private static string GetDifficulty(ulong manager)
        {
            if (Hypervisor.Read<byte>(manager + 0x437F9, true) != 0)
            {
                return "Hard";
            }

            var difficulty = Hypervisor.Read<byte>(manager + 0xB69A0, true);
            var classic = Hypervisor.Read<byte>(manager + 0xB69C4, true) != 0;
            return difficulty switch
            {
                0 => classic ? "Classic (Easy)" : "Easy",
                1 => classic ? "Classic (Normal)" : "Normal",
                2 => "Hard",
                _ => "Unknown"
            };
        }
    }
}

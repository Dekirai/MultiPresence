namespace MultiPresence.Models.DMC5
{
    internal static class Missions
    {
        private static readonly string[] Titles =
        {
            "Prologue",
            "Nero",
            "Qliphoth",
            "Flying Hunter",
            "V",
            "The Devil Sword Sparda",
            "Steel Impact",
            "United Front",
            "Demon King",
            "Genesis",
            "Awaken",
            "Reason",
            "Yamato",
            "Three Warriors",
            "Diverging Point: V",
            "Diverging Point: Nero",
            "Diverging Point: Dante",
            "Brothers",
            "Awakening",
            "Vergil",
            "True Power"
        };

        public static string Title(int mission) => mission >= 0 && mission < Titles.Length ? Titles[mission] : "";
    }
}

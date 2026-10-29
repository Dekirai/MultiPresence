namespace MultiPresence.Models.CCFFVII
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Mode { get; init; } = "";
        public string Character { get; init; } = "";
        public int Level { get; init; }
        public int Hp { get; init; }
        public int MaxHp { get; init; }
        public int Mp { get; init; }
        public int MaxMp { get; init; }
        public int Ap { get; init; }
        public int MaxAp { get; init; }
        public int Sp { get; init; }
        public int Gil { get; init; }
        public string Difficulty { get; init; } = "";
        public int Chapter { get; init; }
        public string ChapterName { get; init; } = "";
        public string Location { get; init; } = "";
        public string Region { get; init; } = "";
        public string Area { get; init; } = "";
        public string MapId { get; init; } = "";
        public string MissionId { get; init; } = "";
        public string MissionName { get; init; } = "";
    }
}

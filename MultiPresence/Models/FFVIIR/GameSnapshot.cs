namespace MultiPresence.Models.FFVIIR
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Campaign { get; init; } = "";
        public string Character { get; init; } = "";
        public int CharacterId { get; init; } = -1;
        public string CharacterKey { get; init; } = "";
        public int Level { get; init; }
        public int Hp { get; init; }
        public int MaxHp { get; init; }
        public int Mp { get; init; }
        public int MaxMp { get; init; }
        public int Sp { get; init; }
        public int? MaxSp { get; init; }
        public int WeaponLevel { get; init; }
        public int? Exp { get; init; }
        public int? MaxExp { get; init; }
        public int TotalExp { get; init; }
        public int? NextExp { get; init; }
        public int Chapter { get; init; }
        public string ChapterName { get; init; } = "";
        public string ChapterId { get; init; } = "";
        public string Location { get; init; } = "";
        public string Region { get; init; } = "";
        public string Map { get; init; } = "";
        public string Area { get; init; } = "";
        public string LocationId { get; init; } = "";
        public string MapId { get; init; } = "";
        public int LayerId { get; init; } = -1;
        public int GroupId { get; init; } = -1;
        public string Difficulty { get; init; } = "";
    }
}

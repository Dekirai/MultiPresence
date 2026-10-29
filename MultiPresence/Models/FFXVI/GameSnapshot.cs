namespace MultiPresence.Models.FFXVI
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Character { get; init; } = "";
        public int Level { get; init; }
        public int Hp { get; init; }
        public int MaxHp { get; init; }
        public int Gil { get; init; }
        public int AbilityPoints { get; init; }
        public string Difficulty { get; init; } = "";
        public int LimitBreak { get; init; }
        public int LimitBreakBars { get; init; }
        public int MaxLimitBreakBars { get; init; }
        public string Location { get; init; } = "";
        public string Region { get; init; } = "";
        public string Area { get; init; } = "";
        public string MapId { get; init; } = "";
        public string Quest { get; init; } = "";
        public string QuestId { get; init; } = "";
        public string QuestObjective { get; init; } = "";
        public string QuestObjectiveId { get; init; } = "";
    }
}

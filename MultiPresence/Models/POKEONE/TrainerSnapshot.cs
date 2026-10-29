namespace MultiPresence.Models.POKEONE
{
    internal sealed record TrainerSnapshot
    {
        public uint? Experience { get; init; }
        public uint? ExpRequired { get; init; }
        public decimal? ExpPercent { get; init; }
        public uint? Money { get; init; }
        public uint? Tokens { get; init; }
        public uint? GameCoins { get; init; }
        public uint? BattlePoints { get; init; }
        public int? BadgeCount { get; init; }
        public string Badges { get; init; } = "";
        public int? PokedexSeen { get; init; }
        public int? PokedexCaught { get; init; }
    }
}

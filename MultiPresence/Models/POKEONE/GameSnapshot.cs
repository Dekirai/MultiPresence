namespace MultiPresence.Models.POKEONE
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Menu";
        public string PlayerName { get; init; } = "";
        public int TrainerLevel { get; init; }
        public string Region { get; init; } = "";
        public string Location { get; init; } = "";
        public bool IsInBattle { get; init; }
        public int HealthyPokemon { get; init; }
        public int TeamSize { get; init; }
        public string BattleType { get; init; } = "";
        public TrainerSnapshot Trainer { get; init; } = new TrainerSnapshot();
        public PokemonSnapshot[] Team { get; init; } = [];
        public PokemonSnapshot[] ActivePokemon { get; init; } = [];
        public PokemonSnapshot[] Opponents { get; init; } = [];
    }
}

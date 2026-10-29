namespace MultiPresence.Models.POKEONE
{
    internal sealed record PokemonSnapshot
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";
        public string Form { get; init; } = "";
        public int Personality { get; init; }
        public int Level { get; init; }
        public int? Health { get; init; }
        public int? MaxHealth { get; init; }
        public string Status { get; init; } = "";
        public bool Shiny { get; init; }
        public bool Active { get; init; }
        public int Position { get; init; }
        public bool Egg { get; init; }
    }
}

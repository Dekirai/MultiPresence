using MultiPresence.Runtime;
using MultiPresence.Infrastructure;
using DiscordRPC;
using MultiPresence.Models.POKEONE;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class POKEONE
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1556140941792903248");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/PokeOne.json");
            PresenceRuntime.Start(nameof(POKEONE), "PokeOne", RPC);
            return Task.CompletedTask;
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("PokeOne");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("POKEONE: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("PokeOne"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("PokeOne"))
                    {
                        ShowStatus("Waiting for game data");
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (snapshot.State != "Ingame" && snapshot.State != "Menu")
                    {
                        ShowStatus(snapshot.State == "Loading" ? "Loading" : "Waiting for game data");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    string template = "Default";
                    if (snapshot.State == "Ingame")
                    {
                        template = snapshot.IsInBattle ? "Battle" : "Ingame";
                    }

                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "PokeOne", placeholders, template);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("PokeOne memory is not ready.", exception);
                        nextWarning = DateTime.UtcNow.AddSeconds(30);
                    }

                    ShowStatus("Waiting for game data");
                }
            }
            else
            {
                MemoryReader.Reset();
                discord.Deinitialize();
                updater.Dispose();
                PresenceRuntime.RequestDetection();
            }
        }

        private static Task<Dictionary<string, object>> GeneratePlaceholders(GameSnapshot snapshot)
        {
            var placeholders = new Dictionary<string, object>
            {
                { "playername", "" },
                { "level", "" },
                { "trainerlevel", "" },
                { "region", "" },
                { "room", "" },
                { "location", "" },
                { "inbattle", "" },
                { "battlestatus", "" },
                { "pokemonremaining", "" },
                { "teamsize", "" },
                { "pokemonfainted", "" },
                { "battletype", "" },
                { "battledetails", "" },
                { "exp", "" },
                { "exprequired", "" },
                { "exppercent", "" },
                { "money", "" },
                { "tokens", "" },
                { "gold", "" },
                { "gamecoins", "" },
                { "battlepoints", "" },
                { "badgecount", "" },
                { "badges", "" },
                { "pokedexseen", "" },
                { "pokedexcaught", "" },
                { "trainerdetails", "" },
                { "team", "" },
                { "teamdetails", "" },
                { "activepokemon", "" },
                { "activepokemondetails", "" },
                { "opponents", "" },
                { "opponentsdetails", "" },
                { "regionimage", "" },
                { "activeimage", "" }
            };
            AddPokemon(placeholders, "pokemon", null);
            AddPokemon(placeholders, "opponent", null);
            for (int position = 1; position <= 6; position++)
            {
                AddPokemon(placeholders, "pokemon" + position, null);
            }

            if (snapshot.State != "Ingame")
            {
                return Task.FromResult(placeholders);
            }

            var location = string.Join(" / ", new[] { snapshot.Region, snapshot.Location }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase));

            placeholders["playername"] = snapshot.PlayerName;
            placeholders["level"] = snapshot.TrainerLevel;
            placeholders["trainerlevel"] = snapshot.TrainerLevel;
            placeholders["region"] = snapshot.Region;
            placeholders["room"] = snapshot.Location;
            placeholders["location"] = location;
            placeholders["inbattle"] = snapshot.IsInBattle;
            placeholders["battlestatus"] = snapshot.IsInBattle ? "In Battle" : "Not in Battle";
            placeholders["pokemonremaining"] = snapshot.HealthyPokemon;
            placeholders["teamsize"] = snapshot.TeamSize;
            placeholders["pokemonfainted"] = snapshot.TeamSize - snapshot.HealthyPokemon;
            placeholders["battletype"] = snapshot.IsInBattle ? snapshot.BattleType : "";
            placeholders["battledetails"] = placeholders["battlestatus"];
            if (snapshot.IsInBattle && snapshot.BattleType.Length != 0)
            {
                placeholders["battledetails"] = snapshot.BattleType.EndsWith("Battle", StringComparison.OrdinalIgnoreCase) ? snapshot.BattleType : snapshot.BattleType + " Battle";
            }
            placeholders["exp"] = (object?)snapshot.Trainer.Experience ?? "";
            placeholders["exprequired"] = (object?)snapshot.Trainer.ExpRequired ?? "";
            placeholders["exppercent"] = snapshot.Trainer.ExpPercent?.ToString("0.##", CultureInfo.InvariantCulture) ?? "";
            placeholders["money"] = (object?)snapshot.Trainer.Money ?? "";
            placeholders["tokens"] = (object?)snapshot.Trainer.Tokens ?? "";
            placeholders["gold"] = (object?)snapshot.Trainer.Tokens ?? "";
            placeholders["gamecoins"] = (object?)snapshot.Trainer.GameCoins ?? "";
            placeholders["battlepoints"] = (object?)snapshot.Trainer.BattlePoints ?? "";
            placeholders["badgecount"] = (object?)snapshot.Trainer.BadgeCount ?? "";
            placeholders["badges"] = snapshot.Trainer.Badges;
            placeholders["pokedexseen"] = (object?)snapshot.Trainer.PokedexSeen ?? "";
            placeholders["pokedexcaught"] = (object?)snapshot.Trainer.PokedexCaught ?? "";
            placeholders["trainerdetails"] = TrainerDetails(snapshot.Trainer);
            placeholders["team"] = string.Join(", ", snapshot.Team.Select(pokemon => pokemon.Name));
            placeholders["teamdetails"] = ShortText(string.Join(" | ", snapshot.Team.Select(PokemonDetails)));
            placeholders["activepokemon"] = string.Join(", ", snapshot.ActivePokemon.Select(pokemon => pokemon.Name));
            placeholders["activepokemondetails"] = ShortText(string.Join(" | ", snapshot.ActivePokemon.Select(PokemonDetails)));
            placeholders["opponents"] = snapshot.IsInBattle ? string.Join(", ", snapshot.Opponents.Select(pokemon => pokemon.Name)) : "";
            placeholders["opponentsdetails"] = snapshot.IsInBattle ? ShortText(string.Join(" | ", snapshot.Opponents.Select(PokemonDetails))) : "";
            var regionImage = PokemonCatalog.GetRegionImageUrl(snapshot.Region);
            placeholders["regionimage"] = regionImage.Length == 0 ? "logo" : regionImage;
            AddPokemon(placeholders, "pokemon", snapshot.ActivePokemon.FirstOrDefault());
            AddPokemon(placeholders, "opponent", snapshot.IsInBattle ? snapshot.Opponents.FirstOrDefault() : null);
            placeholders["activeimage"] = placeholders["pokemonimage"];
            for (int position = 1; position <= 6; position++)
            {
                AddPokemon(placeholders, "pokemon" + position, snapshot.Team.FirstOrDefault(pokemon => pokemon.Position == position));
            }

            return Task.FromResult(placeholders);
        }

        private static void AddPokemon(Dictionary<string, object> placeholders, string prefix, PokemonSnapshot? pokemon)
        {
            placeholders[prefix] = pokemon?.Name ?? "";
            placeholders[prefix + "id"] = pokemon == null ? "" : pokemon.Id;
            placeholders[prefix + "form"] = pokemon?.Form ?? "";
            placeholders[prefix + "level"] = pokemon == null ? "" : pokemon.Level;
            placeholders[prefix + "hp"] = (object?)pokemon?.Health ?? "";
            placeholders[prefix + "maxhp"] = (object?)pokemon?.MaxHealth ?? "";
            placeholders[prefix + "status"] = pokemon?.Status ?? "";
            placeholders[prefix + "shiny"] = pokemon == null ? "" : pokemon.Shiny;
            placeholders[prefix + "active"] = pokemon == null ? "" : pokemon.Active;
            placeholders[prefix + "position"] = pokemon == null ? "" : pokemon.Position;
            placeholders[prefix + "egg"] = pokemon == null ? "" : pokemon.Egg;
            placeholders[prefix + "image"] = pokemon == null || pokemon.Egg ? "" : PokemonCatalog.GetImageUrl(pokemon.Id, pokemon.Shiny, pokemon.Form);
            placeholders[prefix + "details"] = pokemon == null ? "" : PokemonDetails(pokemon);
        }

        private static string PokemonDetails(PokemonSnapshot pokemon)
        {
            if (pokemon.Egg)
            {
                return "Egg";
            }

            var details = new List<string>() { pokemon.Name + " Lv. " + pokemon.Level };
            if (pokemon.Health.HasValue)
            {
                details.Add("HP: " + pokemon.Health + (pokemon.MaxHealth.HasValue ? " / " + pokemon.MaxHealth : ""));
            }
            if (pokemon.Status.Length != 0)
            {
                details.Add(pokemon.Status);
            }
            if (pokemon.Shiny)
            {
                details.Add("Shiny");
            }

            return ShortText(string.Join(" | ", details));
        }

        private static string TrainerDetails(TrainerSnapshot trainer)
        {
            var details = new List<string>();
            if (trainer.Experience.HasValue && trainer.ExpRequired.HasValue)
            {
                details.Add("EXP: " + trainer.Experience + " / " + trainer.ExpRequired);
            }
            if (trainer.Money.HasValue)
            {
                details.Add("Money: " + trainer.Money);
            }
            if (trainer.BattlePoints.HasValue)
            {
                details.Add("BP: " + trainer.BattlePoints);
            }
            if (trainer.BadgeCount.HasValue)
            {
                details.Add("Badges: " + trainer.BadgeCount);
            }
            if (trainer.PokedexCaught.HasValue && trainer.PokedexSeen.HasValue)
            {
                details.Add("Dex: " + trainer.PokedexCaught + " / " + trainer.PokedexSeen);
            }

            return ShortText(string.Join(" | ", details));
        }

        private static string ShortText(string text)
        {
            return text.Length <= 128 ? text : text[..125] + "...";
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("POKEONE: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "PokeOne"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static GameSnapshot ReadSnapshot()
        {
            return MemoryReader.ReadSnapshot();
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("POKEONE: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

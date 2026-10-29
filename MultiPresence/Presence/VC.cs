using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.VC;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class VC
    {
        private const string ProcessName = "Vampire Crawlers";
        private const string GameName = "Vampire Crawlers";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;
        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1562723812523647096");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Vampire Crawlers.json");
            PresenceRuntime.Start(nameof(VC), ProcessName, RPC);
            return Task.CompletedTask;
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(ProcessName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("VC: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning(ProcessName))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach(ProcessName))
                    {
                        ShowStatus("Waiting for game data");
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (snapshot.State is not "Ingame" and not "Menu")
                    {
                        ShowStatus(snapshot.Activity);
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders, snapshot.State == "Ingame" ? "Ingame" : "Default");
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Vampire Crawlers memory is not ready.", exception);
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
            var result = new Dictionary<string, object>
            {
                { "state", snapshot.State },
                { "activity", snapshot.Activity },
                { "character", "" },
                { "mainchar", "" },
                { "characters", "" },
                { "characterid", "" },
                { "characterids", "" },
                { "characterCount", "" },
                { "map", "" },
                { "stage", "" },
                { "location", "" },
                { "mapid", "" },
                { "region", "" },
                { "dungeon", "" },
                { "floor", "" },
                { "completedFloors", "" },
                { "level", "" },
                { "kills", "" },
                { "coins", "" },
                { "health", "" },
                { "maxhealth", "" },
                { "hp", "" },
                { "maxhp", "" },
                { "armor", "" },
                { "mana", "" },
                { "startingMana", "" },
                { "xp", "" },
                { "maxxp", "" },
                { "totalxp", "" },
                { "turn", "" },
                { "combo", "" },
                { "playerTurn", "" },
                { "drawCount", "" },
                { "discardCount", "" },
                { "cardCount", "" },
                { "cards", "" },
                { "cardIds", "" },
                { "modes", "" },
                { "endless", "" },
                { "hurry", "" },
                { "handSizeLimit", "" }
            };
            if (snapshot.State != "Ingame")
            {
                return Task.FromResult(result);
            }

            var mainId = snapshot.CharacterIds.FirstOrDefault() ?? "";
            var character = GameText.GetCharacter(mainId);
            var region = GameText.GetRegion(snapshot.DungeonId);
            var dungeon = GameText.GetDungeon(snapshot.DungeonId);
            var map = string.Join(" / ", new[] { region, dungeon }.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase));
            var modes = new List<string>();
            if (snapshot.Endless)
            {
                modes.Add("Endless");
            }

            if (snapshot.Hurry)
            {
                modes.Add("Hurry");
            }

            if (snapshot.HandSizeLimit)
            {
                modes.Add("Hand Size Limit");
            }

            result["character"] = result["mainchar"] = character;
            result["characters"] = string.Join(", ", snapshot.CharacterIds.Select(GameText.GetCharacter));
            result["characterid"] = mainId;
            result["characterids"] = string.Join(", ", snapshot.CharacterIds);
            result["characterCount"] = snapshot.CharacterIds.Length;
            result["map"] = result["stage"] = result["location"] = map;
            result["mapid"] = snapshot.DungeonId;
            result["region"] = region;
            result["dungeon"] = dungeon;
            result["floor"] = snapshot.Floor;
            result["completedFloors"] = snapshot.CompletedFloors;
            result["level"] = snapshot.Level;
            result["kills"] = snapshot.Kills;
            result["coins"] = snapshot.Coins;
            result["health"] = result["hp"] = Number(snapshot.Health);
            result["maxhealth"] = result["maxhp"] = Number(snapshot.MaxHealth);
            result["armor"] = Number(snapshot.Armor);
            result["mana"] = Number(snapshot.Mana);
            result["startingMana"] = snapshot.StartingMana;
            result["xp"] = snapshot.Experience;
            result["maxxp"] = snapshot.NextLevelExperience;
            result["totalxp"] = snapshot.TotalExperience;
            result["turn"] = snapshot.Turn;
            result["combo"] = snapshot.Combo;
            result["playerTurn"] = snapshot.PlayerTurn;
            result["drawCount"] = snapshot.DrawCount;
            result["discardCount"] = snapshot.DiscardCount;
            result["cardCount"] = snapshot.HandIds.Length;
            result["cards"] = FormatCards(snapshot.HandIds);
            result["cardIds"] = string.Join(", ", snapshot.HandIds);
            result["modes"] = modes.Count == 0 ? "None" : string.Join(", ", modes);
            result["endless"] = snapshot.Endless;
            result["hurry"] = snapshot.Hurry;
            result["handSizeLimit"] = snapshot.HandSizeLimit;
            return Task.FromResult(result);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("VC: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = string.IsNullOrWhiteSpace(text) ? "Waiting for game data" : text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = GameName
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static GameSnapshot ReadSnapshot()
        {
            return MemoryReader.ReadSnapshot();
        }

        private static string Number(float value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private static string FormatCards(string[] ids)
        {
            return ids.Length == 0 ? "None" : string.Join(", ", ids.GroupBy(id => id, StringComparer.Ordinal).Select(group => group.Count() == 1 ? GameText.GetCard(group.Key) : $"{GameText.GetCard(group.Key)} ×{group.Count()}"));
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("VC: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.LR;
using MultiPresence.Runtime;

namespace MultiPresence.Presence
{
    public class LR
    {
        private const string GameName = "Labyrinthine";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1340678671123349546");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Labyrinthine.json");
            PresenceRuntime.Start(nameof(LR), GameName, RPC);
            return Task.CompletedTask;
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(GameName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("LR: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning(GameName))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach(GameName))
                    {
                        ShowStatus("Waiting for game data");
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (snapshot.State == "Loading")
                    {
                        ShowStatus("Loading");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders, snapshot.State);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Labyrinthine memory is not ready.", exception);
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

        private static GameSnapshot ReadSnapshot()
        {
            return MemoryReader.ReadSnapshot();
        }

        private static Task<Dictionary<string, object>> GeneratePlaceholders(GameSnapshot snapshot)
        {
            var placeholders = new Dictionary<string, object>
            {
                { "mode", "" },
                { "level", "" },
                { "session", "" },
                { "players", "" },
                { "playercount", "" },
                { "maxplayers", "" },
                { "chapter", "" },
                { "case", "" },
                { "caseasset", "logo" },
                { "difficulty", "" },
                { "casetype", "" },
                { "mapsize", "" },
                { "lives", "" },
                { "objective", "" },
                { "progress", "" },
                { "map_icon", "logo" }
            };

            if (snapshot.State is not "Lobby" and not "Story" and not "CaseFile")
            {
                return Task.FromResult(placeholders);
            }

            var multiplayer = snapshot.Online || snapshot.PlayerCount > 1;
            var playerCount = snapshot.MaxPlayers > 0 ? $"{snapshot.PlayerCount}/{snapshot.MaxPlayers}" : snapshot.PlayerCount.ToString();
            var mode = snapshot.State == "CaseFile" ? "Case File" : snapshot.State == "Story" ? "Story" : "Lobby";
            var players = multiplayer ? $" | {playerCount} Players" : "";
            placeholders["mode"] = mode;
            placeholders["level"] = snapshot.Level;
            placeholders["session"] = multiplayer ? $"Multiplayer ({playerCount})" : "Singleplayer";
            placeholders["players"] = players;
            placeholders["playercount"] = snapshot.PlayerCount;
            placeholders["maxplayers"] = snapshot.MaxPlayers;
            placeholders["chapter"] = snapshot.Chapter > 0 ? snapshot.Chapter : "";
            placeholders["case"] = snapshot.CaseName;
            placeholders["caseasset"] = snapshot.CaseAsset;
            placeholders["difficulty"] = snapshot.Difficulty;
            placeholders["casetype"] = snapshot.CaseType;
            placeholders["mapsize"] = snapshot.MapSize;
            placeholders["lives"] = snapshot.State == "CaseFile" ? snapshot.Lives : "";
            placeholders["objective"] = snapshot.Objective;
            placeholders["progress"] = snapshot.Progress;
            placeholders["map_icon"] = snapshot.CaseAsset;

            return Task.FromResult(placeholders);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("LR: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = GameName
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("LR: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

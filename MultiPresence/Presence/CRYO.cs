using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.CRYO;
using MultiPresence.Runtime;

namespace MultiPresence.Presence
{
    public class CRYO
    {
        private const string GameName = "Cryo";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1563470423520317513");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Cryo.json");
            PresenceRuntime.Start(nameof(CRYO), GameName, RPC);
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
                throw new InvalidOperationException("CRYO: presence is not initialized.");
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
                        AppLog.Warning("Cryo memory is not ready.", exception);
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
                { "session", "" },
                { "players", "" },
                { "playercount", "" },
                { "maxplayers", "" },
                { "map", "" },
                { "mapasset", "logo" },
                { "difficulty", "" },
                { "objective", "" },
                { "progress", "" }
            };

            if (snapshot.State is not "Lobby" and not "Ingame")
            {
                return Task.FromResult(placeholders);
            }

            var multiplayer = snapshot.Online || snapshot.PlayerCount > 1;
            var playerCount = snapshot.MaxPlayers > 0 ? $"{snapshot.PlayerCount}/{snapshot.MaxPlayers}" : snapshot.PlayerCount.ToString();
            placeholders["session"] = multiplayer ? $"Multiplayer ({playerCount})" : "Singleplayer";
            placeholders["players"] = multiplayer ? $" | {playerCount} Players" : "";
            placeholders["playercount"] = snapshot.PlayerCount;
            placeholders["maxplayers"] = snapshot.MaxPlayers;
            placeholders["map"] = snapshot.Map;
            placeholders["mapasset"] = snapshot.MapAsset;
            placeholders["difficulty"] = snapshot.Difficulty;
            placeholders["objective"] = snapshot.Objective;
            placeholders["progress"] = snapshot.Progress;

            return Task.FromResult(placeholders);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("CRYO: presence is not initialized.");
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
                throw new InvalidOperationException("CRYO: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

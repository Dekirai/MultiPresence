using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.CROC2;
using MultiPresence.Runtime;

namespace MultiPresence.Presence
{
    public class CROC2
    {
        private const string GameName = "Croc 2";
        private const string ProcessName = "croc2";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static void DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1566448655983050842");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Croc 2.json");
            PresenceRuntime.Start(nameof(CROC2), ProcessName, RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(ProcessName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("CROC2: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning(ProcessName))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach(ProcessName))
                    {
                        discord.ClearPresence();
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (snapshot.State != "Ingame")
                    {
                        ShowStatus(snapshot.State == "Loading" ? "Loading" : snapshot.State == "GameOver" ? "Game Over" : "In Main Menu");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Croc 2 memory is not ready.", exception);
                        nextWarning = DateTime.UtcNow.AddSeconds(30);
                    }

                    discord.ClearPresence();
                }
            }
            else
            {
                MemoryReader.Reset();
                discord.ClearPresence();
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
            if (snapshot.State != "Ingame")
            {
                return Task.FromResult(new Dictionary<string, object>());
            }

            return Task.FromResult(new Dictionary<string, object>
            {
                { "health", snapshot.Health },
                { "maxhealth", snapshot.MaxHealth },
                { "bonushearts", snapshot.BonusHearts },
                { "crystals", snapshot.Crystals },
                { "currentcrystals", snapshot.CurrentCrystals },
                { "levelcrystals", snapshot.LevelCrystals },
                { "goldengobbos", snapshot.GoldenGobbos },
                { "keys", snapshot.Keys },
                { "clockworkgobbos", snapshot.ClockworkGobbos },
                { "location", snapshot.Location },
                { "world", snapshot.World },
                { "level", snapshot.Level },
                { "mapid", snapshot.MapId },
                { "areatype", snapshot.AreaType },
                { "gameplaymode", snapshot.GameplayMode },
                { "isoverworld", snapshot.IsOverworld }
            });
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("CROC2: presence is not initialized.");
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
                throw new InvalidOperationException("CROC2: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

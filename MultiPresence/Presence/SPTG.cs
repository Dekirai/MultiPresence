using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.SPTG;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class SPTG
    {
        private const string GameName = "Scott Pilgrim vs The World";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static void DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1394219480871342223");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Scott Pilgrim vs The World.json");
            PresenceRuntime.Start(nameof(SPTG), "scott", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("scott");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("SPTG: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("scott"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("scott"))
                    {
                        discord.ClearPresence();
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (snapshot.State != "Ingame")
                    {
                        ShowStatus(snapshot.State == "Loading" ? "Loading" : "In Menus");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Scott Pilgrim vs The World memory is not ready.", exception);
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

            var money = snapshot.Money.ToString("0.00", CultureInfo.InvariantCulture);
            return Task.FromResult(new Dictionary<string, object>
            {
                { "character", snapshot.Character },
                { "characterid", snapshot.CharacterId },
                { "level", snapshot.Level },
                { "stage", snapshot.Stage },
                { "stageid", snapshot.StageId },
                { "difficulty", snapshot.Difficulty },
                { "difficultyid", snapshot.DifficultyId },
                { "health", snapshot.Health.ToString("0.##", CultureInfo.InvariantCulture) },
                { "gutpoints", snapshot.GutPoints.ToString("0.##", CultureInfo.InvariantCulture) },
                { "defense", snapshot.Defense.ToString("0.##", CultureInfo.InvariantCulture) },
                { "speed", snapshot.Speed.ToString("0.##", CultureInfo.InvariantCulture) },
                { "willpower", snapshot.Willpower.ToString("0.##", CultureInfo.InvariantCulture) },
                { "strength", snapshot.Strength.ToString("0.##", CultureInfo.InvariantCulture) },
                { "lives", snapshot.Lives },
                { "money", money },
                { "coins", money },
                { "totalexperience", snapshot.TotalExperience.ToString("0.##", CultureInfo.InvariantCulture) }
            });
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("SPTG: presence is not initialized.");
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
                throw new InvalidOperationException("SPTG: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

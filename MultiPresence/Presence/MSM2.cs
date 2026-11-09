using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.MSM2;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class MSM2
    {
        private const string GameName = "Marvel's Spider-Man 2";
        private const string ProcessName = "Spider-Man2";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static void DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1337729691951431795");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Marvel's Spider-Man 2.json");
            PresenceRuntime.Start(nameof(MSM2), ProcessName, RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(ProcessName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("MSM2: presence is not initialized.");
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
                        ShowStatus(snapshot.State == "Loading" ? "Loading" : "In Main Menu");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Marvel's Spider-Man 2 memory is not ready.", exception);
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
                { "level", snapshot.Level },
                { "ultimatelevel", snapshot.UltimateLevel },
                { "newgameplus", snapshot.NewGamePlus ? "New Game Plus" : "New Game" },
                { "character", snapshot.Character },
                { "characterid", snapshot.CharacterId },
                { "character_icon", snapshot.CharacterIcon },
                { "location", snapshot.Location },
                { "locationid", snapshot.LocationId },
                { "health", Math.Floor(snapshot.Health).ToString("0", CultureInfo.InvariantCulture) },
                { "maxhealth", Math.Floor(snapshot.MaxHealth).ToString("0", CultureInfo.InvariantCulture) },
                { "skillpoints", snapshot.SkillPoints },
                { "experience", snapshot.Experience },
                { "experience_total", snapshot.TotalExperience },
                { "experience_required", snapshot.NextLevelExperience },
                { "difficulty", snapshot.Difficulty },
                { "difficultyid", snapshot.DifficultyId }
            });
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("MSM2: presence is not initialized.");
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
                throw new InvalidOperationException("MSM2: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

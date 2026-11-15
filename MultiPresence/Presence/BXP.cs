using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.BXP;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class BXP
    {
        private const string ProcessName = "Balls";
        private const string GameName = "BALL x PIT";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1571202130096951326");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/BALL x PIT.json");
            PresenceRuntime.Start(nameof(BXP), ProcessName, RPC);
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
                throw new InvalidOperationException("BXP: presence is not initialized.");
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
                        AppLog.Warning("BALL x PIT memory is not ready.", exception);
                        nextWarning = DateTime.UtcNow.AddSeconds(30);
                    }

                    discord.ClearPresence();
                }
            }
            else
            {
                StopPresence();
            }
        }

        private static GameSnapshot ReadSnapshot()
        {
            return MemoryReader.ReadSnapshot();
        }

        private static Task<Dictionary<string, object>> GeneratePlaceholders(GameSnapshot snapshot)
        {
            string progress = snapshot.CharacterLevel.HasValue ? $"Character Lv. {Number(snapshot.CharacterLevel)}" : "";
            if (progress.Length > 0 && snapshot.Experience.HasValue && snapshot.MaxExperience.HasValue)
                progress += $" | XP {Number(snapshot.Experience)}/{Number(snapshot.MaxExperience)}";
            var placeholders = new Dictionary<string, object>
            {
                { "location", snapshot.Location },
                { "stage", snapshot.Stage },
                { "gold", Number(snapshot.Gold) },
                { "wheat", Number(snapshot.Wheat) },
                { "wood", Number(snapshot.Wood) },
                { "stone", Number(snapshot.Stone) },
                { "character", snapshot.Character },
                { "speed", snapshot.Speed },
                { "floor", Number(snapshot.Layer) },
                { "level", Number(snapshot.Level) },
                { "rungold", Number(snapshot.RunGold) },
                { "health", Number(snapshot.Health) },
                { "maxhealth", Number(snapshot.MaxHealth) },
                { "kills", Number(snapshot.Kills) },
                { "characterlevel", Number(snapshot.CharacterLevel) },
                { "characterprogress", progress },
                { "xp", Number(snapshot.Experience) },
                { "maxxp", Number(snapshot.MaxExperience) }
            };
            return Task.FromResult(placeholders);
        }

        private static string Number(int? value) => value?.ToString(CultureInfo.InvariantCulture) ?? "";

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("BXP: presence is not initialized.");
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

        private static void StopPresence()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("BXP: presence is not initialized.");
            }

            MemoryReader.Reset();
            discord.ClearPresence();
            discord.Deinitialize();
            updater.Dispose();
            PresenceRuntime.RequestDetection();
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("BXP: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

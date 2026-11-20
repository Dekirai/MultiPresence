using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.DMC5;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class DMC5
    {
        private const string ProcessName = "DevilMayCry5";
        private const string GameName = "Devil May Cry 5";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1358118109004828802");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Devil May Cry 5.json");
            PresenceRuntime.Start(nameof(DMC5), ProcessName, RPC);
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
                throw new InvalidOperationException("DMC5: presence is not initialized.");
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
                    if (snapshot.State == "Main Menu")
                    {
                        ShowStatus("In Main Menu");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders, snapshot.State);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Devil May Cry 5 memory is not ready.", exception);
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
            string mission = snapshot.Mission is > 0 ? $"Mission {snapshot.Mission.Value.ToString("00", CultureInfo.InvariantCulture)}" : "";
            string missionDisplay = snapshot.Mode != "Story" ? snapshot.Mode : mission.Length > 0 && snapshot.MissionTitle.Length > 0 ? $"{mission} - {snapshot.MissionTitle}" : snapshot.MissionTitle;
            var placeholders = new Dictionary<string, object>
            {
                { "redorbs", Number(snapshot.RedOrbs) },
                { "blueorbs", Number(snapshot.BlueOrbs) },
                { "purpleorbs", Number(snapshot.PurpleOrbs) },
                { "goldorbs", Number(snapshot.GoldOrbs) },
                { "mission", Number(snapshot.Mission) },
                { "missiontitle", snapshot.MissionTitle },
                { "missiondisplay", missionDisplay },
                { "mode", snapshot.Mode },
                { "difficulty", snapshot.Difficulty },
                { "character", snapshot.Character },
                { "health", Number(snapshot.Health) },
                { "maxhealth", Number(snapshot.MaxHealth) }
            };
            return Task.FromResult(placeholders);
        }

        private static string Number(int? value) => value?.ToString(CultureInfo.InvariantCulture) ?? "";

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("DMC5: presence is not initialized.");
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
                throw new InvalidOperationException("DMC5: presence is not initialized.");
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
                throw new InvalidOperationException("DMC5: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

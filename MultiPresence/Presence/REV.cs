using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.REV;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class REV
    {
        private const string ProcessName = "rerev";
        private const string GameName = "Resident Evil Revelations";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1571590244606681168");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Resident Evil Revelations.json");
            PresenceRuntime.Start(nameof(REV), ProcessName, RPC);
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
                throw new InvalidOperationException("REV: presence is not initialized.");
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
                        AppLog.Warning("Resident Evil Revelations memory is not ready.", exception);
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
            string difficulty = snapshot.State == "Campaign"
                ? snapshot.DifficultyId switch { 0 => "Casual", 1 => "Normal", 2 => "Infernal", _ => "" }
                : snapshot.DifficultyId switch { 0 => "Chasm", 1 => "Trench", 2 => "Abyss", _ => "" };
            string chapter = snapshot.Episode.HasValue ? $"Episode {Number(snapshot.Episode)}" : "";
            if (chapter.Length > 0 && snapshot.EpisodeTitle.Length > 0)
                chapter += $" - {snapshot.EpisodeTitle}";
            string stage = snapshot.Stage;
            if (stage.Length > 0 && snapshot.StageName.Length > 0)
                stage += $" - {snapshot.StageName}";
            var placeholders = new Dictionary<string, object>
            {
                { "character", snapshot.Character },
                { "partner", snapshot.Partner },
                { "partnerdisplay", snapshot.Partner.Length > 0 ? $" | Partner: {snapshot.Partner}" : "" },
                { "episode", Number(snapshot.Episode) },
                { "episodetitle", snapshot.EpisodeTitle },
                { "chapter", chapter },
                { "difficulty", difficulty },
                { "difficultydisplay", difficulty.Length > 0 ? $" | {difficulty}" : "" },
                { "location", snapshot.Location },
                { "locationdisplay", snapshot.Location.Length > 0 ? $" | {snapshot.Location}" : "" },
                { "objective", snapshot.Objective },
                { "stage", stage },
                { "stagenumber", snapshot.Stage },
                { "stagename", snapshot.StageName },
                { "level", Number(snapshot.Level) },
                { "leveldisplay", snapshot.Level.HasValue ? $" (Lv. {Number(snapshot.Level)})" : "" }
            };
            return Task.FromResult(placeholders);
        }

        private static string Number(int? value) => value?.ToString(CultureInfo.InvariantCulture) ?? "";

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("REV: presence is not initialized.");
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
                throw new InvalidOperationException("REV: presence is not initialized.");
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
                throw new InvalidOperationException("REV: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

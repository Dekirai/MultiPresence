using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.FFRS;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class FFRS
    {
        private const string ProcessName = "FFRS-Win64-Shipping";
        private const string GameName = "FINAL FANTASY RESONANCE";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1572684677343609012");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/FINAL FANTASY RESONANCE.json");
            PresenceRuntime.Start(nameof(FFRS), ProcessName, RPC);
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
                throw new InvalidOperationException("FFRS: presence is not initialized.");
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
                        AppLog.Warning("FINAL FANTASY RESONANCE memory is not ready.", exception);
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
            string chapter = snapshot.Chapter.Length > 0 && snapshot.ChapterTitle.Length > 0 ? $"{snapshot.Chapter} - {snapshot.ChapterTitle}" : snapshot.ChapterTitle;
            var placeholders = new Dictionary<string, object>
            {
                { "character", snapshot.Character },
                { "party", snapshot.Party },
                { "partydisplay", snapshot.Party.Length > 0 ? $" | Party: {snapshot.Party}" : "" },
                { "level", Number(snapshot.Level) },
                { "nextlevel", Number(snapshot.NextLevel) },
                { "hp", Number(snapshot.Health) },
                { "hpmax", Number(snapshot.MaxHealth) },
                { "mp", Number(snapshot.Mana) },
                { "mpmax", Number(snapshot.MaxMana) },
                { "lb", Number(snapshot.LimitBurst) },
                { "gil", Number(snapshot.Gil) },
                { "difficulty", snapshot.Difficulty },
                { "chapter", chapter },
                { "chaptertitle", snapshot.ChapterTitle },
                { "objective", snapshot.Objective },
                { "location", snapshot.Location }
            };
            return Task.FromResult(placeholders);
        }

        private static string Number(int? value) => value?.ToString(CultureInfo.InvariantCulture) ?? "";

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("FFRS: presence is not initialized.");
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
                throw new InvalidOperationException("FFRS: presence is not initialized.");
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
                throw new InvalidOperationException("FFRS: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

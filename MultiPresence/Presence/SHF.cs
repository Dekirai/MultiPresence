using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.SHF;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class SHF
    {
        private const string ProcessName = "SHf-Win64-Shipping";
        private const string GameName = "SILENT HILL f";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1459970389064290304");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/SILENT HILL f.json");
            PresenceRuntime.Start(nameof(SHF), ProcessName, RPC);
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
                throw new InvalidOperationException("SHF: presence is not initialized.");
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
                        AppLog.Warning("SILENT HILL f memory is not ready.", exception);
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
            var placeholders = new Dictionary<string, object>
            {
                { "location", "" },
                { "areaid", "" },
                { "objective", "" },
                { "objectivename", "" },
                { "objectivecount", "" },
                { "objectivemaxcount", "" },
                { "health", "" },
                { "maxhealth", "" },
                { "healthpercentage", "" },
                { "stamina", "" },
                { "maxstamina", "" },
                { "staminapercentage", "" }
            };

            if (snapshot.State == "Ingame")
            {
                placeholders["location"] = snapshot.Location;
                placeholders["areaid"] = snapshot.AreaId;
                placeholders["objective"] = snapshot.Objective;
                placeholders["objectivename"] = snapshot.ObjectiveName;
                placeholders["objectivecount"] = snapshot.ObjectiveCurrent?.ToString(CultureInfo.InvariantCulture) ?? "";
                placeholders["objectivemaxcount"] = snapshot.ObjectiveMax?.ToString(CultureInfo.InvariantCulture) ?? "";
                placeholders["health"] = Format(snapshot.Health);
                placeholders["maxhealth"] = Format(snapshot.MaxHealth);
                placeholders["healthpercentage"] = Percentage(snapshot.Health, snapshot.MaxHealth);
                placeholders["stamina"] = Format(snapshot.Stamina);
                placeholders["maxstamina"] = Format(snapshot.MaxStamina);
                placeholders["staminapercentage"] = Percentage(snapshot.Stamina, snapshot.MaxStamina);
            }
            return Task.FromResult(placeholders);
        }

        private static string Format(float? value) => value?.ToString("0.#", CultureInfo.InvariantCulture) ?? "";

        private static string Percentage(float? value, float? maximum) => maximum > 0 ? Format(value * 100 / maximum) : "";

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("SHF: presence is not initialized.");
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
                throw new InvalidOperationException("SHF: presence is not initialized.");
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
                throw new InvalidOperationException("SHF: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

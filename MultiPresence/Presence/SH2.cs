using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.SH2;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class SH2
    {
        private const string ProcessName = "SHProto-Win64-Shipping";
        private const string GameName = "SILENT HILL 2";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1569367946139148430");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/SILENT HILL 2.json");
            PresenceRuntime.Start(nameof(SH2), ProcessName, RPC);
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
                throw new InvalidOperationException("SH2: presence is not initialized.");
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
                        AppLog.Warning("SILENT HILL 2 memory is not ready.", exception);
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
                { "area", "" },
                { "areaid", "" },
                { "room", "" },
                { "health", "" },
                { "maxhealth", "" },
                { "healthpercentage", "" },
                { "healthstatus", "" },
                { "weapon", "" },
                { "ammo", "" },
                { "reserveammo", "" },
                { "weaponammo", "" },
                { "combatdifficulty", "" },
                { "puzzledifficulty", "" }
            };

            if (snapshot.State == "Ingame")
            {
                placeholders["location"] = snapshot.Location;
                placeholders["area"] = snapshot.Area;
                placeholders["areaid"] = snapshot.AreaId;
                placeholders["room"] = snapshot.Room;
                placeholders["health"] = Format(snapshot.Health);
                placeholders["maxhealth"] = Format(snapshot.MaxHealth);
                placeholders["healthpercentage"] = Percentage(snapshot.Health, snapshot.MaxHealth);
                placeholders["healthstatus"] = snapshot.HealthStatus;
                placeholders["weapon"] = snapshot.Weapon;
                placeholders["ammo"] = snapshot.Ammo?.ToString(CultureInfo.InvariantCulture) ?? "";
                placeholders["reserveammo"] = snapshot.ReserveAmmo?.ToString(CultureInfo.InvariantCulture) ?? "";
                placeholders["weaponammo"] = FormatWeapon(snapshot);
                placeholders["combatdifficulty"] = snapshot.CombatDifficulty;
                placeholders["puzzledifficulty"] = snapshot.PuzzleDifficulty;
            }
            return Task.FromResult(placeholders);
        }

        private static string FormatWeapon(GameSnapshot snapshot)
        {
            if (snapshot.Ammo is not int loaded) return snapshot.Weapon;
            return snapshot.ReserveAmmo is int reserve
                ? $"{snapshot.Weapon} ({loaded}/{reserve})" : $"{snapshot.Weapon} ({loaded})";
        }

        private static string Format(float? value) => value?.ToString("0.#", CultureInfo.InvariantCulture) ?? "";

        private static string Percentage(float? value, float? maximum) => maximum > 0 ? Format(value * 100 / maximum) : "";

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("SH2: presence is not initialized.");
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
                throw new InvalidOperationException("SH2: presence is not initialized.");
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
                throw new InvalidOperationException("SH2: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.SR1;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class SR1
    {
        private const string ProcessName = "SRX";
        private const string GameName = "Legacy of Kain Soul Reaver 1";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1563488496893558804");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Legacy of Kain Soul Reaver 1.json");
            PresenceRuntime.Start(nameof(SR1), ProcessName, RPC);
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
                throw new InvalidOperationException("SR1: presence is not initialized.");
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
                    if (snapshot.State == "OtherGame")
                    {
                        StopPresence();
                        return;
                    }

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
                        AppLog.Warning("Soul Reaver 1 memory is not ready.", exception);
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
                { "character", "" },
                { "location", "" },
                { "areaid", "" },
                { "realm", "" },
                { "objective", "" },
                { "health", "" },
                { "maxhealth", "" },
                { "healthpercentage", "" },
                { "mana", "" },
                { "maxmana", "" },
                { "healthupgrades", "" },
                { "energyupgrades", "" }
            };

            if (snapshot.State != "Ingame")
            {
                return Task.FromResult(placeholders);
            }

            placeholders["character"] = "Raziel";
            placeholders["location"] = snapshot.Location;
            placeholders["areaid"] = snapshot.Area;
            placeholders["realm"] = snapshot.Realm;
            placeholders["objective"] = snapshot.Objective;
            placeholders["health"] = snapshot.Health;
            placeholders["maxhealth"] = snapshot.MaxHealth;
            placeholders["healthpercentage"] = snapshot.MaxHealth > 0
                ? Math.Round(snapshot.Health * 100.0 / snapshot.MaxHealth, 1, MidpointRounding.AwayFromZero).ToString("0.#", CultureInfo.InvariantCulture)
                : "";
            placeholders["mana"] = snapshot.Mana;
            placeholders["maxmana"] = snapshot.MaxMana;
            placeholders["healthupgrades"] = snapshot.HealthUpgrades;
            placeholders["energyupgrades"] = snapshot.EnergyUpgrades;

            return Task.FromResult(placeholders);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("SR1: presence is not initialized.");
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
                throw new InvalidOperationException("SR1: presence is not initialized.");
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
                throw new InvalidOperationException("SR1: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

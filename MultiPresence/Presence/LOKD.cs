using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.LOKD;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class LOKD
    {
        private const string ProcessName = "defiance";
        private const string GameName = "Legacy of Kain: Defiance Remastered";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1569355364712054825");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Legacy of Kain Defiance Remastered.json");
            PresenceRuntime.Start(nameof(LOKD), ProcessName, RPC);
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
                throw new InvalidOperationException("LOKD: presence is not initialized.");
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
                        AppLog.Warning("Defiance Remastered memory is not ready.", exception);
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
                { "chapter", "" },
                { "location", "" },
                { "area", "" },
                { "room", "" },
                { "areaid", "" },
                { "realm", "" },
                { "reaver", "" },
                { "reaverlevel", "" },
                { "reaverxp", "" },
                { "health", "" },
                { "maxhealth", "" },
                { "healthpercentage", "" },
                { "objective", "" },
                { "primaryobjective", "" },
                { "secondaryobjective", "" }
            };

            if (snapshot.State != "Ingame")
            {
                return Task.FromResult(placeholders);
            }

            placeholders["character"] = snapshot.Character;
            placeholders["chapter"] = snapshot.Chapter >= 0 ? snapshot.Chapter : "";
            placeholders["location"] = snapshot.Location;
            placeholders["area"] = snapshot.Area;
            placeholders["room"] = snapshot.Room;
            placeholders["areaid"] = snapshot.AreaId;
            placeholders["realm"] = snapshot.Realm;
            placeholders["reaver"] = snapshot.Reaver;
            placeholders["reaverlevel"] = snapshot.ReaverLevel >= 0 ? snapshot.ReaverLevel : "";
            placeholders["reaverxp"] = snapshot.ReaverXp >= 0 ? snapshot.ReaverXp : "";
            placeholders["health"] = snapshot.Health >= 0 ? snapshot.Health : "";
            placeholders["maxhealth"] = snapshot.MaxHealth >= 0 ? snapshot.MaxHealth : "";
            placeholders["objective"] = snapshot.Objective;
            placeholders["primaryobjective"] = snapshot.PrimaryObjective;
            placeholders["secondaryobjective"] = snapshot.SecondaryObjective;
            placeholders["healthpercentage"] = snapshot.MaxHealth > 0 && snapshot.Health >= 0
                ? Math.Round(snapshot.Health * 100.0 / snapshot.MaxHealth, 1, MidpointRounding.AwayFromZero).ToString("0.#", CultureInfo.InvariantCulture)
                : "";

            return Task.FromResult(placeholders);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("LOKD: presence is not initialized.");
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
                throw new InvalidOperationException("LOKD: presence is not initialized.");
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
                throw new InvalidOperationException("LOKD: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

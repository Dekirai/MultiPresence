using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.DDV;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class DDV
    {
        private const string ProcessName = "ddv";
        private const string GameName = "Disney Dreamlight Valley";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1563798798738923562");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Disney Dreamlight Valley.json");
            PresenceRuntime.Start(nameof(DDV), ProcessName, RPC);
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
                throw new InvalidOperationException("DDV: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning(ProcessName))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach(ProcessName))
                    {
                        ShowStatus("Waiting for game data");
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
                        AppLog.Warning("Disney Dreamlight Valley memory is not ready.", exception);
                        nextWarning = DateTime.UtcNow.AddSeconds(30);
                    }

                    ShowStatus("Waiting for game data");
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
            var placeholders = new Dictionary<string, object>
            {
                { "location", "" },
                { "quest", "" },
                { "objective", "" },
                { "money", "" },
                { "playername", "" },
                { "level", "" },
                { "energy", "" },
                { "maxenergy", "" },
                { "dreamlight", "" },
                { "locationdetails", "" }
            };

            if (snapshot.State != "Ingame")
            {
                return Task.FromResult(placeholders);
            }

            placeholders["location"] = snapshot.Location;
            placeholders["quest"] = snapshot.Quest;
            placeholders["objective"] = snapshot.Objective;
            placeholders["money"] = (object?)snapshot.Money ?? "";
            placeholders["playername"] = snapshot.PlayerName;
            placeholders["level"] = (object?)snapshot.Level ?? "";
            placeholders["energy"] = (object?)snapshot.Energy ?? "";
            placeholders["maxenergy"] = (object?)snapshot.MaxEnergy ?? "";
            placeholders["dreamlight"] = (object?)snapshot.Dreamlight ?? "";
            var locationDetails = new List<string>();
            if (!string.IsNullOrWhiteSpace(snapshot.Location))
            {
                locationDetails.Add(snapshot.Location);
            }
            if (snapshot.Money.HasValue)
            {
                locationDetails.Add(snapshot.Money.Value.ToString("N0", CultureInfo.InvariantCulture) + " Star Coins");
            }
            placeholders["locationdetails"] = string.Join(" | ", locationDetails);

            return Task.FromResult(placeholders);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("DDV: presence is not initialized.");
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
                throw new InvalidOperationException("DDV: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}

#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.MSMR;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MSMR
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1266485584822796331");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Marvel's Spider-Man Remastered.json");
            PresenceRuntime.Start(nameof(MSMR), "Spider-Man", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("Spider-Man");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("Spider-Man"))
            {
                float health_get = Hypervisor.Read<float>(0x6D302D8);
                int health = (int)Math.Floor(health_get);

                if (health > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Marvel's Spider-Man Remastered", placeholders);
                }
                else
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Marvel's Spider-Man Remastered"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                PresenceRuntime.RequestDetection();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int level = Hypervisor.Read<byte>(0x5DB60A0);
            int location_get = Hypervisor.Read<byte>(0x5DC06D0);
            float health_get = Hypervisor.Read<float>(0x6D302D8);
            var location = await Locations.GetLocations(location_get);
            int health = (int)Math.Floor(health_get);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "health", health },
                { "location", location }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
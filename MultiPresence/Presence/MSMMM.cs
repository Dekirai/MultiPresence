#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.MSMMM;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MSMMMM
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;

        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1266464310360670241");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Marvel's Spider-Man Miles Morales.json");
            PresenceRuntime.Start(nameof(MSMMMM), "MilesMorales", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("MilesMorales");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("MilesMorales"))
            {
                float health_get = Hypervisor.Read<float>(0x7796D68);
                int health = (int)Math.Floor(health_get);

                if (health > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Marvel's Spider-Man: Miles Morales", placeholders);
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
                            LargeImageText = "Marvel's Spider-Man: Miles Morales"
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
            int level = Hypervisor.Read<byte>(0x671CA70);
            int location_get = Hypervisor.Read<byte>(0x6724900);
            float health_get = Hypervisor.Read<float>(0x7796D68);
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

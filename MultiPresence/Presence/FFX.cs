#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFX
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async Task DoAction()
        {
            await Task.Delay(10000);
            GetPID();
            discord = new DiscordRpcClient("1457341286649565339");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Final Fantasy X.json");
            PresenceRuntime.Start(nameof(FFX), "FFX", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("FFX");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("FFX"))
            {
                string location_get = Hypervisor.ReadString(Hypervisor.GetPointer32(0x008E81E4, [0xD0]), 255, true);

                if (location_get != "More detailed description")
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy X", placeholders);
                }
                else
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In menues",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Final Fantasy X"
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
            string location_get = Hypervisor.ReadString(Hypervisor.GetPointer32(0x008E81E4, [0xD0]), 255, true);
            location_get = location_get.Replace("\0", "");

            int idx = location_get.IndexOf("Play Time:", StringComparison.OrdinalIgnoreCase);

            string location = (idx >= 0 ? location_get.Substring(0, idx) : location_get)
                .TrimEnd('\r', '\n', ' ').Replace("Location: ", String.Empty);
            int gil = Hypervisor.Read<int>(0xD307D8);
            int slv = Hypervisor.Read<short>(0xD32097);

            return new Dictionary<string, object>
            {
                { "slv", slv  },
                { "gil", gil },
                { "location", location }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
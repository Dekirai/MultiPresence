using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFX
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static async void DoAction()
        {
            await Task.Delay(10000);
            GetPID();
            discord = new DiscordRpcClient("1457341286649565339");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Final Fantasy X.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("FFX")[0];
                if (_myProcess.Id > 0)
                    Hypervisor.AttachProcess(_myProcess);
            }
            catch
            {
                //nothing?
            }
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName("FFX");
            if (game.Length > 0)
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

                await Task.Delay(3000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                MainForm.gameUpdater.Start();
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
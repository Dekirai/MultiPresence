using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class CB4
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1395058700892377190");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Crash Bandicoot 4.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("CrashBandicoot4")[0];
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
            Process[] game = Process.GetProcessesByName("CrashBandicoot4");
            if (game.Length > 0)
            {
                int destroyablecrates = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x158, 0x8, 0x430]), true);

                try
                {
                    if (destroyablecrates > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Crash Bandicoot 4: It's About Time", placeholders);
                    }
                    else
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Menus",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Crash Bandicoot 4: It's About Time"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                }
                catch
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Menus",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Crash Bandicoot 4: It's About Time"
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
            int destroyablecrates = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x158, 0x8, 0x430]), true);
            int crates = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x158, 0x8, 0x438]), true);
            int deaths = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x158, 0x8, 0x5A8]), true);
            int wumpasretro = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x30, 0xA8, 0x70, 0x3F8]), true);
            int wumpasmodern = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x30, 0xA8, 0x70, 0x408]), true);

            return new Dictionary<string, object>
            {
                { "maxcrates", destroyablecrates },
                { "currentcrates", crates },
                { "deaths", deaths },
                { "wumpasretro", wumpasretro },
                { "wumpasmodern", wumpasmodern }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}

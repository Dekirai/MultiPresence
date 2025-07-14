using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class SPTG
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1394219480871342223");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Scott Pilgrim vs The World.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("scott")[0];
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
            Process[] game = Process.GetProcessesByName("scott");
            if (game.Length > 0)
            {
                try
                {
                    ulong _base = Hypervisor.GetPointer64(0x0230FF48, [0xC8, 0x1A8, 0x178]);
                    float health = Hypervisor.Read<float>(_base, true);

                    if (health > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Scott Pilgrim vs The World", placeholders);
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
                                LargeImageText = "Scott Pilgrim vs The World"
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
                            LargeImageText = "Scott Pilgrim vs The World"
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
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            ulong _base = Hypervisor.GetPointer64(0x0230FF48, [0xC8, 0x1A8, 0x178]);
            float health = Hypervisor.Read<float>(_base, true);
            float gutpoints = Hypervisor.Read<float>(_base + 0x1BC, true);
            float defense = Hypervisor.Read<float>(_base + 0x6F0, true);
            float speed = Hypervisor.Read<float>(_base + 0x6F4, true);
            float willpower = Hypervisor.Read<float>(_base + 0x6F8, true);
            float strength = Hypervisor.Read<float>(_base + 0x6FC, true);
            int lives = Hypervisor.Read<int>(_base + 0x700, true);
            float money = Hypervisor.Read<float>(_base + 0x1278, true);
            float totalexperience = Hypervisor.Read<float>(_base + 0x127C, true);

            int money_rounded = (int)Math.Round(money, 0, MidpointRounding.AwayFromZero);

            return new Dictionary<string, object>
            {
                { "health", health },
                { "gutpoints", gutpoints },
                { "defense", defense },
                { "speed", speed },
                { "willpower", willpower },
                { "strength", strength },
                { "lives", lives },
                { "money", money_rounded },
                { "totalexperience", totalexperience }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
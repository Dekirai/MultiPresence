using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class ER
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1344571255453515849");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("eldenring")[0];
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
            Process[] game = Process.GetProcessesByName("eldenring");
            if (game.Length > 0)
            {
                int maxhp = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x03B12E30, [0x0, 0x190, 0x0, 0x13C]), true);

                try
                {
                    if (maxhp > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Elden Ring", placeholders);
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
                                LargeImageText = "Elden Ring"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                }
                catch
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Elden Ring"
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
            string nickname = Hypervisor.ReadString(Hypervisor.GetPointer64(0x03D5DF38, [0x8, 0x9C]), 20, true, null, true);
            int clearcount = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x03D5DF38, [0x120]), true);
            int archetype = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x03D5DF38, [0x8, 0xBF]), true);
            int deaths = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x03D5DF38, [0x94]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x03D5DF38, [0x8, 0x68]), true);
            int souls = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x03D5DF38, [0x8, 0x6C]), true);
            int hp = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x03B12E30, [0x0, 0x190, 0x0, 0x138]), true);
            int maxhp = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x03B12E30, [0x0, 0x190, 0x0, 0x13C]), true);
            int mp = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x03B12E30, [0x0, 0x190, 0x0, 0x148]), true);
            int maxmp = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x03B12E30, [0x0, 0x190, 0x0, 0x150]), true);
            int stamina = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x03B12E30, [0x0, 0x190, 0x0, 0x154]), true);
            int maxstamina = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x03B12E30, [0x0, 0x190, 0x0, 0x158]), true);

            string classname = archetype switch
            {
                0 => "Vagabond",
                1 => "Warrior",
                2 => "Hero",
                3 => "Bandit",
                4 => "Astrologer",
                5 => "Prophet",
                6 => "Confessor",
                7 => "Samurai",
                8 => "Prisoner",
                9 => "Wretch"
            };

            return new Dictionary<string, object>
            {
                { "nickname", nickname },
                { "clearcount", clearcount },
                { "deaths", deaths },
                { "class", classname },
                { "level", level },
                { "souls", souls },
                { "hp", hp },
                { "maxhp", maxhp },
                { "mp", mp },
                { "maxmp", maxmp },
                { "stamina", stamina },
                { "maxstamina", maxstamina }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}

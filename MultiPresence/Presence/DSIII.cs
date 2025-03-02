using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DSIII
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1344011934206529618");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("DarkSoulsIII")[0];
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
            Process[] game = Process.GetProcessesByName("DarkSoulsIII");
            if (game.Length > 0)
            {
                int maxhp = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0x94]), true);

                try
                {
                    if (maxhp > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Dark Souls III", placeholders);
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
                                LargeImageText = "Dark Souls III"
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
                            LargeImageText = "Dark Souls III"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }

                await Task.Delay(1000);
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
            string nickname = Hypervisor.ReadString(Hypervisor.GetPointer64(0x047572B8, [0x10, 0x88]), 20, true, null, true);
            int clearcount = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x047572B8, [0x78]), true);
            int archetype = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x016148F0, [0xA8, 0xC0, 0x64]), true);
            int deaths = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x047572B8, [0x98]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x047572B8, [0x10, 0x70]), true);
            int souls = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x047572B8, [0x10, 0x74]), true);
            int hp = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0x90]), true);
            int maxhp = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0x94]), true);
            int mp = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0x9C]), true);
            int maxmp = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0xA0]), true);
            int stamina = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0xA8]), true);
            int maxstamina = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0xAC]), true);

            string classname = archetype switch
            {
                0 => "Knight",
                1 => "Mercenary",
                2 => "Warrior",
                3 => "Herald",
                4 => "Thief",
                5 => "Assassin",
                6 => "Sorcerer",
                7 => "Pyromancer",
                8 => "Cleric",
                9 => "Deprived",
                10 => "Debug"
            };

            return new Dictionary<string, object>
            {
                { "nickname", nickname },
                { "clearcount", clearcount },
                { "class", classname },
                { "deaths", deaths },
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

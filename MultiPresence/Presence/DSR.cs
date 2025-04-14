using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DSR
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1343654813866528900");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Dark Souls I.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("DarkSoulsRemastered")[0];
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
            Process[] game = Process.GetProcessesByName("DarkSoulsRemastered");
            if (game.Length > 0)
            {
                int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C77E50, [0x68, 0x3EC]), true);

                try
                {
                    if (maxhp > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Dark Souls: Remastered", placeholders);
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
                                LargeImageText = "Dark Souls: Remastered"
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
                            LargeImageText = "Dark Souls: Remastered"
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
            string nickname = Hypervisor.ReadString(Hypervisor.GetPointer64(0x01C7C5F0, [0x218, 0x1074]), 20, true, null, true);
            int clearcount = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C7C5F0, [0x218, 0x2B0]), true);
            int archetype = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x01C7C5F0, [0x218, 0x1016]), true);
            int deaths = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C7C5F0, [0x218, 0x2D0]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C7C5F0, [0x218, 0xFD8]), true);
            int souls = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C7C5F0, [0x218, 0xFDC]), true);
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C77E50, [0x68, 0x3E8]), true);
            int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C77E50, [0x68, 0x3EC]), true);
            int mp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C77E50, [0x68, 0x3F0]), true);
            int maxmp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C77E50, [0x68, 0x3F4]), true);
            int stamina = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C77E50, [0x68, 0x3F8]), true);
            int maxstamina = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01C77E50, [0x68, 0x3FC]), true);

            string classname = archetype switch
            {
                0 => "Warrior",
                1 => "Knight",
                2 => "Wanderer",
                3 => "Thief",
                4 => "Bandit",
                5 => "Hunter",
                6 => "Sorcerer",
                7 => "Pyromancer",
                8 => "Cleric",
                9 => "Deprived"
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

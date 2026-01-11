using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class SMT5
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1458883872250007562");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Shin Megami Tensei V.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("SMT5V-Win64-Shipping")[0];
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
            Process[] game = Process.GetProcessesByName("SMT5V-Win64-Shipping");
            if (game.Length > 0)
            {
                try
                {
                    int level = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x0453AA80, [0x58, 0x2A0, 0x2C8]), true);
                    if (level == 0 || level == 1)
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Main Menu",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Shin Megami Tensei V: Vengeance"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                    else
                    {
                        int isBattle = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x040D4620, [0x300, 0x788]), true);
                        if (isBattle == 0)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Shin Megami Tensei V", placeholders);
                        }

                        else if (isBattle == 1)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersBattle);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Shin Megami Tensei V", placeholders, "Battle");
                        }
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
                            LargeImageText = "Shin Megami Tensei V: Vengeance"
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
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x0453AA80, [0x58, 0x298]), true);
            int money = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0453AA80, [0x58, 0x2F0]), true);
            int glory = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0453AA80, [0x58, 0x314]), true);
            int level = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x0453AA80, [0x58, 0x2A0, 0x2C8]), true);
            string difficulty = difficulty_get switch
            {
                0 => "Safety",
                1 => "Casual",
                2 => "Normal",
                3 => "Hard",
                _ => "Unknown",
            };

            return new Dictionary<string, object>
            {
                { "difficulty", difficulty },
                { "money", money },
                { "glory", glory },
                { "level", level }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersBattle()
        {
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x0453AA80, [0x58, 0x298]), true);
            int money = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0453AA80, [0x58, 0x2F0]), true);
            int glory = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0453AA80, [0x58, 0x314]), true);
            int level = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x0453AA80, [0x58, 0x2A0, 0x2C8]), true);
            int hp_battle = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x042F1570, [0x1A0, 0xA0, 0xB0, 0x94]), true);
            int hpmax_battle = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x042F1570, [0x1A0, 0xA0, 0xB0, 0x98]), true);
            int mp_battle = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x042F1570, [0x1A0, 0xA0, 0xB0, 0x9C]), true);
            int mpmax_battle = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x042F1570, [0x1A0, 0xA0, 0xB0, 0xA0]), true);
            string difficulty = difficulty_get switch
            {
                0 => "Safety",
                1 => "Casual",
                2 => "Normal",
                3 => "Hard",
                _ => "Unknown",
            };

            return new Dictionary<string, object>
            {
                { "difficulty", difficulty },
                { "money", money },
                { "glory", glory },
                { "level", level },
                { "hp", hp_battle },
                { "hpmax", hpmax_battle },
                { "mp", mp_battle },
                { "mpmax", mpmax_battle }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
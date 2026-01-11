using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class SMT3
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1459518206002855988");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Shin Megami Tensei III.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("smt3hd")[0];
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
            Process[] game = Process.GetProcessesByName("smt3hd");
            if (game.Length > 0)
            {
                try
                {
                    int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x02E4DC78, [0xB8, 0x28, 0x58, 0x20, 0x24], false, "GameAssembly.dll"), true);
                    if (level == 0)
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Main Menu",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Shin Megami Tensei III Nocturne"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                    else
                    {
                        //int isBattle = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x02CE5D98, [0x50, 0xDD0, 0x28, 0x178, 0x68, 0x1D0, 0xF30], false, "GameAssembly.dll"), true);
                        int isBattle = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x02E7FBA8, [0x30, 0x38, 0x30, 0x388, 0x110, 0x60, 0x90], false, "GameAssembly.dll"), true);

                        if (isBattle == 0)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Shin Megami Tensei III", placeholders);
                        }
                        else if (isBattle == 1)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersBattle);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Shin Megami Tensei III", placeholders, "Battle");
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
                            LargeImageText = "Shin Megami Tensei III Nocturne"
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
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x02E445E0, [0x80, 0xA8, 0xB8, 0x48, 0x20, 0x28, 0x24], false, "GameAssembly.dll"), true);
            int money = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x02E4ED30, [0xB8, 0x0, 0x48], false, "GameAssembly.dll"), true);
            int magatama_get = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x02E4ED30, [0xB8, 0x0, 0xA2], false, "GameAssembly.dll"), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x02E4DC78, [0xB8, 0x28, 0x58, 0x20, 0x24], false, "GameAssembly.dll"), true);

            string magatama = magatama_get switch
            {
                0 => "None",
                1 => "Marogareh",
                2 => "Wadatsumi",
                3 => "Ankh",
                4 => "Iyomante",
                5 => "Shiranui",
                6 => "Hifumi",
                7 => "Kamudo",
                8 => "Narukami",
                9 => "Anathema",
                10 => "Miasma",
                11 => "Nirvana",
                12 => "Murakumo",
                13 => "Geis",
                14 => "Djed",
                15 => "Muspell",
                16 => "Gehenna",
                17 => "Kamurogi",
                18 => "Satan",
                19 => "Adama",
                20 => "Vimana",
                21 => "Gundari",
                22 => "Sophia",
                23 => "Gaea",
                24 => "Kailash",
                25 => "Masakados",
                _ => "Unknown",
            };

            string difficulty = difficulty_get switch
            {
                0 => "Merciful",
                1 => "Normal",
                2 => "Hard",
                _ => "Unknown",
            };

            return new Dictionary<string, object>
            {
                { "difficulty", difficulty },
                { "magatama", magatama },
                { "money", money },
                { "level", level }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersBattle()
        {
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x02E445E0, [0x80, 0xA8, 0xB8, 0x48, 0x20, 0x28, 0x24], false, "GameAssembly.dll"), true);
            int money = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x02E4ED30, [0xB8, 0x0, 0x48], false, "GameAssembly.dll"), true);
            int magatama_get = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x02E4ED30, [0xB8, 0x0, 0xA2], false, "GameAssembly.dll"), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x02E4DC78, [0xB8, 0x28, 0x58, 0x20, 0x24], false, "GameAssembly.dll"), true);
            string magatama = magatama_get switch
            {
                0 => "None",
                1 => "Marogareh",
                2 => "Wadatsumi",
                3 => "Ankh",
                4 => "Iyomante",
                5 => "Shiranui",
                6 => "Hifumi",
                7 => "Kamudo",
                8 => "Narukami",
                9 => "Anathema",
                10 => "Miasma",
                11 => "Nirvana",
                12 => "Murakumo",
                13 => "Geis",
                14 => "Djed",
                15 => "Muspell",
                16 => "Gehenna",
                17 => "Kamurogi",
                18 => "Satan",
                19 => "Adama",
                20 => "Vimana",
                21 => "Gundari",
                22 => "Sophia",
                23 => "Gaea",
                24 => "Kailash",
                25 => "Masakados",
                _ => "Unknown",
            };

            string difficulty = difficulty_get switch
            {
                0 => "Merciful",
                1 => "Normal",
                2 => "Hard",
                _ => "Unknown",
            };

            return new Dictionary<string, object>
            {
                { "difficulty", difficulty },
                { "magatama", magatama },
                { "money", money },
                { "level", level }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
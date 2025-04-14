using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC3
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static async void DoAction()
        {
            await Task.Delay(5000);
            GetPID();
            discord = new DiscordRpcClient("1358354576310534247");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Devil May Cry 3.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("dmc3")[0];
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
            Process[] game = Process.GetProcessesByName("dmc3");
            if (game.Length > 0)
            {
                float maxhealth = Hypervisor.Read<float>(0x046DE36C, true);
                int menuflag = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x00C90DF8, [0x40, 0x20]), true);
                int mission = Hypervisor.Read<byte>(0xC8F250);

                if (menuflag == 1)
                {
                    if (mission == 21)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersBP);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 3", placeholders, "Bloody Palace");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 3", placeholders);
                    }
                }
                else if (menuflag == 2)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 3", placeholders, "Cutscene");
                }
                else if (menuflag == 3)
                {
                    if (mission == 21)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersBP);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 3", placeholders, "Bloody Palace Pause Menu");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 3", placeholders, "Pause Menu");
                    }
                }
                else if (menuflag == 4)
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Status/Mission start",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Devil May Cry 3"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }
                else if (menuflag == 6)
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "Game Over",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Devil May Cry 3"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }
                else if (menuflag == 10)
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Save Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Devil May Cry 3"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }
                else if (menuflag == 12)
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Mission Select",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Devil May Cry 3"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
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
                            LargeImageText = "Devil May Cry 3"
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
            float health = Hypervisor.Read<float>(0x046DE39C, true);
            float maxhealth = Hypervisor.Read<float>(0x046DE36C, true);
            int redorbs = Hypervisor.Read<int>(0x006E0710, true);
            int difficulty_get = Hypervisor.Read<byte>(0xC8F25C);
            int isHeavenOrHell = Hypervisor.Read<byte>(0xC8F260);
            int character_get = Hypervisor.Read<byte>(0xC8F264);
            int mission = Hypervisor.Read<byte>(0xC8F250);

            string difficulty = difficulty_get switch
            {
                0 => "Easy",
                1 => "Normal",
                2 => "Hard",
                3 => "Very Hard",
                4 => "Dante Must Die"
            };
            string character = character_get switch
            {
                0 => "Dante",
                1 => "Vergil",
                2 => "Lady",
                3 => "Vergil",
            };

            if (isHeavenOrHell == 1)
            {
                difficulty = "Heaven or Hell";
            }

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "mission", mission },
                { "difficulty", difficulty },
                { "character", character },
                { "health", health },
                { "maxhealth", maxhealth }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersBP()
        {
            float health = Hypervisor.Read<float>(0x046DE39C, true);
            float maxhealth = Hypervisor.Read<float>(0x046DE36C, true);
            int redorbs = Hypervisor.Read<int>(0x006E0710, true);
            int character_get = Hypervisor.Read<byte>(0xC8F264);
            int level = Hypervisor.Read<int>(0xCB89A4);

            string character = character_get switch
            {
                0 => "Dante",
                1 => "Vergil",
                2 => "Lady",
                3 => "Vergil",
            };

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "character", character },
                { "health", health },
                { "maxhealth", maxhealth },
                { "level", level }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
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
            await Task.Delay(20000);
            GetPID();
            discord = new DiscordRpcClient("1358354576310534247");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
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

                if (maxhealth > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 3", placeholders);
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

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
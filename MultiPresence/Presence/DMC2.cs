using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC2
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static async void DoAction()
        {
            await Task.Delay(20000);
            GetPID();
            discord = new DiscordRpcClient("1358481854235414598");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("dmc2")[0];
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
            Process[] game = Process.GetProcessesByName("dmc2");
            if (game.Length > 0)
            {
                float maxhealth = Hypervisor.Read<float>(0x046DE36C, true);
                int menuflag = Hypervisor.Read<byte>(0x1588C10);

                if (menuflag == 3)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 2", placeholders);
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
                            LargeImageText = "Devil May Cry 2"
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
            int health = Hypervisor.Read<short>(0x158A470);
            int maxhealth = Hypervisor.Read<short>(0x158A474);
            int redorbs = Hypervisor.Read<int>(0x1588BA8);
            int difficulty_get = Hypervisor.Read<byte>(0x15884A0);
            int mission = Hypervisor.Read<byte>(0x157D289);

            string difficulty = difficulty_get switch
            {
                0 => "Normal",
                1 => "Hard",
                2 => "Dante Must Die",
                3 => "Bloody Palace"
            };

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "mission", mission },
                { "difficulty", difficulty },
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
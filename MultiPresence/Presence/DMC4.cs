using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC4
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static async void DoAction()
        {
            await Task.Delay(20000);
            GetPID();
            discord = new DiscordRpcClient("1358474125336772771");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("DevilMayCry4SpecialEdition")[0];
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
            Process[] game = Process.GetProcessesByName("DevilMayCry4SpecialEdition");
            if (game.Length > 0)
            {
                float health = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x00ED8ADC, [0x284, 0x30, 0x284]), true);

                if (health > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 4", placeholders);
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
                            LargeImageText = "Devil May Cry 4"
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
            float health = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x00ED8ADC, [0x284, 0x30, 0x284]), true);
            int redorbs = Hypervisor.Read<int>(Hypervisor.GetPointer32(0xEDEEC4, [0x184]), true);
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x00EEEED0, [0x20]), true);
            int scenario_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xEDEEC4, [0x1BC]), true);
            int mission = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xEDEEC4, [0x150]), true);

            string difficulty = difficulty_get switch
            {
                0 => "Human",
                1 => "Devil Hunter",
                2 => "Son of Sparda",
                3 => "Dante Must Die",
                4 => "Legendary Dark Knight",
                5 => "Heaven or Hell",
                6 => "Hell and Hell"
            };
            string scenario = scenario_get switch
            {
                0 => "Nero/Dante",
                1 => "Vergil",
                2 => "Lady/Trish"
            };

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "mission", mission },
                { "difficulty", difficulty },
                { "scenario", scenario },
                { "health", health },
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
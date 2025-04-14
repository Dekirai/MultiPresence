using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC1
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static async void DoAction()
        {
            await Task.Delay(5000);
            GetPID();
            discord = new DiscordRpcClient("1358367799285649418");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Devil May Cry 1.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("dmc1")[0];
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
            Process[] game = Process.GetProcessesByName("dmc1");
            if (game.Length > 0)
            {
                uint healthdata = Hypervisor.GetPointer32(0x5EAB88, [0x4571]);
                int maxhealth = Hypervisor.Read<short>(healthdata + 0x427, true);
                int menuflag = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x5EAB88, [0x1CA5]), true);

                if (menuflag == 0xA0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry", placeholders);
                }
                else if (menuflag == 0x80)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry", placeholders, "Pause Menu");
                }
                else if (menuflag == 0xA1)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry", placeholders, "Shop Menu");
                }
                else if (menuflag == 0xA4)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry", placeholders, "Mission End");
                }
                else if (menuflag == 0x2C || menuflag == 0x28)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry", placeholders, "Mission Start");
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
                            LargeImageText = "Devil May Cry"
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
            uint healthdata = Hypervisor.GetPointer32(0x5EAB88, [0x4571]);

            int health = Hypervisor.Read<short>(healthdata + 0x1, true);
            int maxhealth = Hypervisor.Read<short>(healthdata + 0x427, true);
            int redorbs = Hypervisor.Read<int>(0x001378FC, true);
            int difficulty_get = Hypervisor.Read<byte>(0x27C0826);
            int mission = Hypervisor.Read<byte>(0x27C0824);

            string difficulty = difficulty_get switch
            {
                3 => "Normal",
                5 => "Hard",
                6 => "Dante Must Die"
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
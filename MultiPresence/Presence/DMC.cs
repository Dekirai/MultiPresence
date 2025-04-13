using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static async void DoAction()
        {
            await Task.Delay(5000);
            GetPID();
            discord = new DiscordRpcClient("1358513563446022235");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("DMC-DevilMayCry")[0];
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
            Process[] game = Process.GetProcessesByName("DMC-DevilMayCry");
            if (game.Length > 0)
            {
                float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x26C31FC, [0x184, 0x14, 0x14, 0x844]), true);
                int bp_level = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x026B5338, [0xAFC, 0x48]), true);
                int mode = Hypervisor.Read<byte>(0x27546CC);

                if (bp_level > 0 && mode == 8)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersBP);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "DmC Devil May Cry", placeholders, "Bloody Palace");
                }
                else if (maxhealth > 0 && mode < 7)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "DmC Devil May Cry", placeholders);
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
                            LargeImageText = "DmC Devil May Cry"
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
            float health = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x26C31FC, [0x184, 0x14, 0x14, 0x840]), true);
            float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x26C31FC, [0x184, 0x14, 0x14, 0x844]), true);
            int redorbs = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x275B0AC, [0x9F4, 0xC]), true);
            int difficulty_get = Hypervisor.Read<byte>(0x27546CC);
            int mission = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x26CE348, [0x24, 0x9C]), true) + 1;

            string difficulty = difficulty_get switch
            {
                0 => "Human",
                1 => "Devil Hunter",
                2 => "Nephilim",
                3 => "Son of Sparda",
                4 => "Dante Must Die",
                5 => "Heaven or Hell",
                6 => "Hell and Hell"
            };

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "mission", mission },
                { "difficulty", difficulty },
                { "health", health },
                { "maxhealth", maxhealth },
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersBP()
        {
            float health = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x26C3070, [0x3C, 0xDC, 0x4, 0x840]), true);
            float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x26C3070, [0x3C, 0xDC, 0x4, 0x844]), true);
            int redorbs = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x275B0AC, [0x9F4, 0xC]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x026B5338, [0xAFC, 0x48]), true);

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
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
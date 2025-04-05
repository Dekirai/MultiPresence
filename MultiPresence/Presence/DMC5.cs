using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC5
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static async void DoAction()
        {
            await Task.Delay(20000);
            GetPID();
            discord = new DiscordRpcClient("1358118109004828802");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("DevilMayCry5")[0];
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
            Process[] game = Process.GetProcessesByName("DevilMayCry5");
            if (game.Length > 0)
            {
                float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x7E6A7F0, [0x88, 0x310, 0x20, 0x88, 0x14]), true);

                if (maxhealth > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 5", placeholders);
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
                            LargeImageText = "Devil May Cry 5"
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
            float health = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x7E6A7F0, [0x88, 0x310, 0x20, 0x88, 0x10]), true);
            float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x7E6A7F0, [0x88, 0x310, 0x20, 0x88, 0x14]), true);
            int redorbs = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x7E5FD40, [0x60]), true);
            int blueorbs = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x7E5FD40, [0x78]), true);
            int purpleorbs = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x7E5FD40, [0x88]), true);
            int goldorbs = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x7E5FD40, [0x98]), true);
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x07E661B0, [0x88]), true);
            int character_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x07E661B0, [0xBC]), true);
            int mission = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x07E661B0, [0x80]), true);

            string difficulty = difficulty_get switch
            {
                0 => "Human",
                1 => "Devil Hunter",
                2 => "Son of Sparda",
                3 => "Dante Must Die",
                4 => "Heaven or Hell",
                5 => "Hell or Hell"
            };
            string character = character_get switch
            {
                0 => "Nero",
                1 => "Dante",
                2 => "V",
                3 => "Vergil",
                4 => "Sunbeam" //?
            };

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "blueorbs", blueorbs },
                { "purpleorbs", purpleorbs },
                { "goldorbs", goldorbs },
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
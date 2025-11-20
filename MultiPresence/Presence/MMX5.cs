using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMX5
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1434181145234374837");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man X5.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("RXC2")[0];
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
            Process[] game = Process.GetProcessesByName("RXC2");
            if (game.Length > 0)
            {
                int _game = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x0338ED04, [0x90]), true);
                if (_game != 0)
                {
                    discord.Deinitialize();
                    MainForm.gameUpdater.Start();
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man X5", placeholders);

                    await Task.Delay(3000);
                    Thread thread = new Thread(RPC);
                    thread.Start();
                }
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
            int _lives = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x033F7444, [0x99]), true);
            int _stage_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x033F7444, [0x64]), true);
            int _character_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x033F7444, [0x9D]), true);

            string character = _character_get switch
            {
                0 => "X",
                1 => "Fourth Armor",
                2 => "Falcon Armor",
                3 => "Gaea Armor",
                4 => "Ultimate Armor",
                5 => "Zero",
                _ => "Maverick Hunter"
            };

            string stage = _stage_get switch
            {
                0 => "Intro",
                1 => "Grizzly Slash Stage",
                2 => "Dark Dizzy Stage",
                3 => "Duff McWhalen Stage",
                4 => "Mattrex Stage",
                5 => "Squid Adler Stage",
                6 => "Izzy Glow Stage",
                7 => "Axle The Red Stage",
                8 => "The Skiver Stage",
                9 => "Dynamo Stage",
                11 => "Cutscene",
                12 => "Zero Virus 4 Stage",
                13 => "Stage select",
                14 => "Title screen",
                15 => "Result screen",
                16 => "Zero Virus 1 Stage",
                17 => "Zero Virus 2 Stage",
                18 => "Zero Virus 3 Stage",
                22 => "Training Stage",
                _ => "Unknown"
            };

            return new Dictionary<string, object>
            {
                { "lives", _lives },
                { "stage", stage },
                { "character", character }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}

using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMX7
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1434314908643889322");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man X7.json");
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
                if (_game != 2)
                {
                    discord.Deinitialize();
                    MainForm.gameUpdater.Start();
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man X7", placeholders);

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
            int _lives = Hypervisor.Read<byte>(0x3DF7A14);
            int _stage_get = Hypervisor.Read<byte>(0x3DF78CD);
            int _character_get = Hypervisor.Read<byte>(0x3DF78D2);

            string character = _character_get switch
            {
                0 => "X",
                1 => "Zero",
                2 => "Axl",
                _ => "Maverick Hunter"
            };

            string stage = _stage_get switch
            {
                0 => "Intro Stage",
                1 => "Intro Stage",
                2 => "Lava Factory",
                3 => "Tunnel Base",
                4 => "Radio Tower",
                5 => "Battleship",
                6 => "Deep Forest",
                7 => "Air Forces",
                8 => "Cyber Field",
                9 => "Central Circuit",
                11 => "Palace Road",
                12 => "Crimson Palace",
                255 => "Stage select",
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

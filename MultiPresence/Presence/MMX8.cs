using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMX8
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
                if (_game != 3)
                {
                    discord.Deinitialize();
                    MainForm.gameUpdater.Start();
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man X8", placeholders);

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
            int _metals = Hypervisor.Read<byte>(0x4209FFC);
            int _stage_get = Hypervisor.Read<byte>(0x420D02C);
            int _character_get = Hypervisor.Read<byte>(0x32A9AA0);

            string character = _character_get switch
            {
                0 => "X",
                1 => "Zero",
                2 => "Axl",
                3 => "Ultimate Armor",
                4 => "Black Zero",
                5 => "White Axl",
                6 => "Alia",
                7 => "Layer",
                8 => "Pallette",
                _ => "Maverick Hunter"
            };

            string stage = _stage_get switch
            {
                0 => "Noah's Park",
                1 => "Troia Base",
                2 => "Primrose",
                3 => "Pitch Black",
                4 => "Dynasty",
                5 => "Inferno",
                6 => "Central White",
                7 => "Metal Valley",
                8 => "Booster Forest",
                9 => "Jakob",
                10 => "Gateway",
                11 => "Sigma Palace",
                255 => "Stage select",
                _ => "Unknown"
            };

            return new Dictionary<string, object>
            {
                { "metals", _metals },
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

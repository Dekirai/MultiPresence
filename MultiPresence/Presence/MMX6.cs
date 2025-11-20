using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMX6
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1434202150510006314");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man X6.json");
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
                if (_game != 1)
                {
                    discord.Deinitialize();
                    MainForm.gameUpdater.Start();
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man X6", placeholders);

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
            int _lives = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x03DD7814, [0xE5]), true);
            int _stage_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x03DD7814, [0xB8]), true);
            int _character_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x03DD7814, [0x10A]), true);

            string character = _character_get switch
            {
                0 => "X",
                1 => "Falcon Armor",
                2 => "Shadow Armor",
                3 => "Blade Armor",
                4 => "Ultimate Armor",
                5 => "Zero",
                _ => "Maverick Hunter"
            };

            string stage = _stage_get switch
            {
                0 => "Intro Stage",
                1 => "Amazon Area",
                2 => "Northpole Area",
                3 => "Magma Area",
                4 => "Recycle Lab",
                5 => "Central Museum",
                6 => "Inami Temple",
                7 => "Laser Institute",
                8 => "Weapon Center",
                9 => "Dynamo Stage",
                11 => "Cutscene",
                12 => "Secret Lab 3",
                13 => "Stage select",
                14 => "Title screen",
                15 => "Result screen",
                16 => "Secret Lab",
                17 => "Secret Lab 2",
                18 => "Secret Lab 2 - Part 2",
                22 => "Sub-Stage",
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

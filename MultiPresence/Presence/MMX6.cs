#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMX6
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1434202150510006314");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man X6.json");
            PresenceRuntime.Start(nameof(MMX6), "RXC2", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("RXC2");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("RXC2"))
            {
                int _game = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x0338ED04, [0x90]), true);
                if (_game != 1)
                {
                    discord.Deinitialize();
                    updater.Dispose();
                    PresenceRuntime.RequestDetection();
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man X6", placeholders);
                }
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                PresenceRuntime.RequestDetection();
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

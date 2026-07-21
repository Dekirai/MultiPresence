#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMX8
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1434314908643889322");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man X7.json");
            PresenceRuntime.Start(nameof(MMX8), "RXC2", RPC);
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
                if (_game != 3)
                {
                    discord.Deinitialize();
                    updater.Dispose();
                    PresenceRuntime.RequestDetection();
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man X8", placeholders);
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

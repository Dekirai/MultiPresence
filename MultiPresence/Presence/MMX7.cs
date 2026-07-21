#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMX7
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1434314908643889322");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man X7.json");
            PresenceRuntime.Start(nameof(MMX7), "RXC2", RPC);
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
                if (_game != 2)
                {
                    discord.Deinitialize();
                    updater.Dispose();
                    PresenceRuntime.RequestDetection();
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man X7", placeholders);
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

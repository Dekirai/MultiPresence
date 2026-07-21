#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.MMBN5;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMBN4
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1435202230553673728");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man Battle Network 4.json");
            PresenceRuntime.Start(nameof(MMBN4), "MMBN_LC2", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("MMBN_LC2");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("MMBN_LC2"))
            {
                int _game = Hypervisor.Read<byte>(0xABEF0A0);
                if (_game != 5 && _game != 6)
                {
                    discord.Deinitialize();
                    updater.Dispose();
                    PresenceRuntime.RequestDetection();
                }
                else
                {
                    int gamestate = Hypervisor.Read<byte>(0x80219FF2, true);

                    if (gamestate == 12)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man Battle Network 4", placeholders, "In_Battle");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man Battle Network 4", placeholders);
                    }
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
            int area_get = Hypervisor.Read<byte>(0x80205FF4, true);
            int room_get = Hypervisor.Read<byte>(0x80205FF5, true);
            int hp = Hypervisor.Read<sbyte>(0x8020B4E4, true);
            int maxhp = Hypervisor.Read<sbyte>(0x8020B4E6, true);
            int gamestate = Hypervisor.Read<byte>(0x80219FF2, true);
            var location = await Areas.GetArea(area_get);

            int _game = Hypervisor.Read<byte>(0xABEF0A0);

            string gameicon = _game switch
            {
                6 => "redsun",
                7 => "bluemoon",
                _ => "logo"
            };

            string gamename = _game switch
            {
                7 => "Mega Man Battle Network 4: Red Sun",
                8 => "Mega Man Battle Network 4: Blue Moon",
                _ => "Mega Man Battle Network 4"
            };

            return new Dictionary<string, object>
            {
                { "hp", hp },
                { "maxhp", maxhp },
                { "gameicon", gameicon },
                { "gamename", gamename }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}

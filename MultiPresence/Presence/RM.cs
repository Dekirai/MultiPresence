#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RM
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1472904074314645658");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Rayman.json");
            PresenceRuntime.Start(nameof(RM), "rayman30th", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("rayman30th");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("rayman30th"))
            {
                int islevel = Hypervisor.Read<byte>(0x80001CEE81, true);

                if (islevel == 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Rayman", placeholders);
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Rayman", placeholders, "Ingame");
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
            int stage = Hypervisor.Read<byte>(0x80001E6330, true);
            int healthstatus = Hypervisor.Read<byte>(0x80001F6200, true);
            int maxhealth = Hypervisor.Read<byte>(0x80001E4D59, true);
            int lives = Hypervisor.Read<byte>(0x80001E4D50, true);
            int tinks = Hypervisor.Read<byte>(0x80001E4D56, true);

            string stagename = stage switch
            {
                0 => "Pink Plant Woods",
                1 => "Anguish Lagoon",
                2 => "The Swamps of Forgetfulness",
                3 => "Moskito's Nest",
                4 => "Bongo Hills",
                5 => "Allegro Presto",
                6 => "Gong Heights",
                7 => "Mr Sax's Hullaballo",
                8 => "Twilight Gulch",
                9 => "The Hard Rocks",
                10 => "Mr Stone's Peaks",
                11 => "Eraser Plains",
                12 => "Pencil Pentathlon",
                13 => "Space Mama's Crater",
                14 => "Crystal Palace",
                15 => "Eat at Joe's",
                16 => "Mr Skops' Stalactites",
                17 => "Mr Dark's Dare",
                _ => "Unknown"
            };

            string health = "";
            if (maxhealth == 4)
            {
                if (healthstatus == 0)
                    health = "🟡🔴🔴🔴🔴";
                else if (healthstatus == 1)
                    health = "🟡🟡🔴🔴🔴";
                else if (healthstatus == 2)
                    health = "🟡🟡🟡🔴🔴";
                else if (healthstatus == 3)
                    health = "🟡🟡🟡🟡🔴";
                else if (healthstatus == 4)
                    health = "🟡🟡🟡🟡🟡";
            }
            else
            {
                if (healthstatus == 0)
                    health = "🟡🔴🔴";
                else if (healthstatus == 1)
                    health = "🟡🟡🔴";
                else if (healthstatus == 2)
                    health = "🟡🟡🟡";
            }


            return new Dictionary<string, object>
            {
                { "level", stagename },
                { "health", health },
                { "lives", lives },
                { "tinks", tinks}
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
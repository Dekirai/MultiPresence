#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async Task DoAction()
        {
            await Task.Delay(5000);
            GetPID();
            discord = new DiscordRpcClient("1358513563446022235");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/DmC Devil May Cry.json");
            PresenceRuntime.Start(nameof(DMC), "DMC-DevilMayCry", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("DMC-DevilMayCry");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("DMC-DevilMayCry"))
            {
                float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x02735220, [0x0, 0x238, 0x844]), true);
                int bp_level = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x026B5338, [0xAFC, 0x48]), true);
                int mode = Hypervisor.Read<byte>(0x27546CC);

                if (bp_level > 0 && mode == 8)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersBP);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "DmC Devil May Cry", placeholders, "Bloody Palace");
                }
                else if (maxhealth > 0 && mode < 7)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "DmC Devil May Cry", placeholders);
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
                            LargeImageText = "DmC Devil May Cry"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
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
            float health = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x02735220, [0x0, 0x238, 0x840]), true);
            float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x02735220, [0x0, 0x238, 0x844]), true);
            int redorbs = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x275B0AC, [0x9F4, 0xC]), true);
            int difficulty_get = Hypervisor.Read<byte>(0x27546CC);
            int mission = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x02733378, [0x7D4, 0x9C]), true) + 1;

            string difficulty = difficulty_get switch
            {
                0 => "Human",
                1 => "Devil Hunter",
                2 => "Nephilim",
                3 => "Son of Sparda",
                4 => "Dante Must Die",
                5 => "Heaven or Hell",
                6 => "Hell and Hell",
                _ => "Unknown"
            };

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "mission", mission },
                { "difficulty", difficulty },
                { "health", health },
                { "maxhealth", maxhealth },
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersBP()
        {
            float health = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x26C3070, [0x3C, 0xDC, 0x4, 0x840]), true);
            float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x26C3070, [0x3C, 0xDC, 0x4, 0x844]), true);
            int redorbs = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x275B0AC, [0x9F4, 0xC]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x026B5338, [0xAFC, 0x48]), true);

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "health", health },
                { "maxhealth", maxhealth },
                { "level", level }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC5
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async Task DoAction()
        {
            await Task.Delay(5000);
            GetPID();
            discord = new DiscordRpcClient("1358118109004828802");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Devil May Cry 5.json");
            PresenceRuntime.Start(nameof(DMC5), "DevilMayCry5", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("DevilMayCry5");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("DevilMayCry5"))
            {
                float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x07E6A7F0, [0x88, 0x1F8, 0x260, 0x10, 0x20, 0x14]), true);

                if (maxhealth > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 5", placeholders);
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
                            LargeImageText = "Devil May Cry 5"
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
            float health = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x07E6A7F0, [0x88, 0x1F8, 0x260, 0x10, 0x20, 0x10]), true);
            float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x07E6A7F0, [0x88, 0x1F8, 0x260, 0x10, 0x20, 0x14]), true);
            int redorbs = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x7E5FD40, [0x60]), true);
            int blueorbs = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x7E5FD40, [0x78]), true);
            int purpleorbs = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x7E5FD40, [0x88]), true);
            int goldorbs = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x7E5FD40, [0x98]), true);
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x07E661B0, [0x88]), true);
            int character_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x07E661B0, [0xBC]), true);
            int mission = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x07E661B0, [0x80]), true);

            string difficulty = difficulty_get switch
            {
                0 => "Human",
                1 => "Devil Hunter",
                2 => "Son of Sparda",
                3 => "Dante Must Die",
                4 => "Heaven or Hell",
                5 => "Hell and Hell",
                _ => "Unknown"
            };
            string character = character_get switch
            {
                0 => "Nero",
                1 => "Dante",
                2 => "V",
                3 => "Vergil",
                4 => "Vergil",
                _ => "Unknown"
            };

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "blueorbs", blueorbs },
                { "purpleorbs", purpleorbs },
                { "goldorbs", goldorbs },
                { "mission", mission },
                { "difficulty", difficulty },
                { "character", character },
                { "health", health },
                { "maxhealth", maxhealth }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
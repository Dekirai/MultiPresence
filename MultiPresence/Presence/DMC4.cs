#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC4
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async Task DoAction()
        {
            await Task.Delay(5000);
            GetPID();
            discord = new DiscordRpcClient("1358474125336772771");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Devil May Cry 4.json");
            PresenceRuntime.Start(nameof(DMC4), "DevilMayCry4SpecialEdition", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("DevilMayCry4SpecialEdition");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("DevilMayCry4SpecialEdition"))
            {
                float health = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x00ED8ADC, [0x284, 0x30, 0x284]), true);

                if (health > 0)
                {
                    int mission = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xEDEEC4, [0x150]), true);
                    if (mission == 50)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersBP);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 4", placeholders, "Bloody Palace");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 4", placeholders);
                    }
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
                            LargeImageText = "Devil May Cry 4"
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
            float health = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x00ED8ADC, [0x284, 0x30, 0x284]), true);
            int redorbs = Hypervisor.Read<int>(Hypervisor.GetPointer32(0xEDEEC4, [0x184]), true);
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x00EEEED0, [0x20]), true);
            int scenario_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xEDEEC4, [0x1BC]), true);
            int mission = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xEDEEC4, [0x150]), true);

            string difficulty = difficulty_get switch
            {
                0 => "Human",
                1 => "Devil Hunter",
                2 => "Son of Sparda",
                3 => "Dante Must Die",
                4 => "Legendary Dark Knight",
                5 => "Heaven or Hell",
                6 => "Hell and Hell",
                _ => "Unknown"
            };
            string scenario = scenario_get switch
            {
                0 => "Nero/Dante",
                1 => "Vergil",
                2 => "Lady/Trish",
                _ => "Unknown"
            };

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "mission", mission },
                { "difficulty", difficulty },
                { "scenario", scenario },
                { "health", health }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersBP()
        {
            float health = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x00ED8ADC, [0x284, 0x30, 0x284]), true);
            int redorbs = Hypervisor.Read<int>(Hypervisor.GetPointer32(0xEDEEC4, [0x184]), true);
            int scenario_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xEDEEC4, [0x1BC]), true);
            int mission = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xEDEEC4, [0x150]), true);
            int level = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xE7E340, [0xC4, 0x180]), true);

            string scenario = scenario_get switch
            {
                0 => "Nero/Dante",
                1 => "Vergil",
                2 => "Lady/Trish",
                _ => "Unknown"
            };

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "mission", mission },
                { "scenario", scenario },
                { "health", health },
                { "level", level }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
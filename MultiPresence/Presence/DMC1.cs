#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC1
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async Task DoAction()
        {
            await Task.Delay(5000);
            GetPID();
            discord = new DiscordRpcClient("1358367799285649418");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Devil May Cry 1.json");
            PresenceRuntime.Start(nameof(DMC1), "dmc1", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("dmc1");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("dmc1"))
            {
                uint healthdata = Hypervisor.GetPointer32(0x5EAB88, [0x4571]);
                int maxhealth = Hypervisor.Read<short>(healthdata + 0x427, true);
                int menuflag = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x5EAB88, [0x1CA5]), true);

                if (menuflag == 0xA0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry", placeholders);
                }
                else if (menuflag == 0x80)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry", placeholders, "Pause Menu");
                }
                else if (menuflag == 0xA1)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry", placeholders, "Shop Menu");
                }
                else if (menuflag == 0xA4)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry", placeholders, "Mission End");
                }
                else if (menuflag == 0x2C || menuflag == 0x28)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry", placeholders, "Mission Start");
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
                            LargeImageText = "Devil May Cry"
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
            uint healthdata = Hypervisor.GetPointer32(0x5EAB88, [0x4571]);

            int health = Hypervisor.Read<short>(healthdata + 0x1, true);
            int maxhealth = Hypervisor.Read<short>(healthdata + 0x427, true);
            int redorbs = Hypervisor.Read<int>(0x001378FC, true);
            int difficulty_get = Hypervisor.Read<byte>(0x27C0826);
            int mission = Hypervisor.Read<byte>(0x27C0824);

            string difficulty = difficulty_get switch
            {
                3 => "Normal",
                5 => "Hard",
                6 => "Dante Must Die",
                _ => "Unknown"
            };

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "mission", mission },
                { "difficulty", difficulty },
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
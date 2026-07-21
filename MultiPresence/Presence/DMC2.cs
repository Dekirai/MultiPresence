#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC2
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async Task DoAction()
        {
            await Task.Delay(5000);
            GetPID();
            discord = new DiscordRpcClient("1358481854235414598");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Devil May Cry 2.json");
            PresenceRuntime.Start(nameof(DMC2), "dmc2", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("dmc2");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("dmc2"))
            {
                float maxhealth = Hypervisor.Read<float>(0x046DE36C, true);
                int menuflag = Hypervisor.Read<byte>(0x1588C10);

                if (menuflag == 3)
                {
                    int difficulty_get = Hypervisor.Read<byte>(0x15884A0);

                    if (difficulty_get == 3)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersBP);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 2", placeholders, "Bloody Palace");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry 2", placeholders);
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
                            LargeImageText = "Devil May Cry 2"
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
            int health = Hypervisor.Read<short>(0x158A470);
            int maxhealth = Hypervisor.Read<short>(0x158A474);
            int redorbs = Hypervisor.Read<int>(0x1588BA8);
            int difficulty_get = Hypervisor.Read<byte>(0x15884A0);
            int mission = Hypervisor.Read<byte>(0x157D289);

            string difficulty = difficulty_get switch
            {
                0 => "Normal",
                1 => "Hard",
                2 => "Dante Must Die",
                3 => "Bloody Palace",
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

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersBP()
        {
            int health = Hypervisor.Read<short>(0x158A470);
            int maxhealth = Hypervisor.Read<short>(0x158A474);
            int redorbs = Hypervisor.Read<int>(0x1588BA8);
            int level = Hypervisor.Read<int>(0x1588C28) + 1;

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
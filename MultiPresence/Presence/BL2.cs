#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class BL2
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1486344469862420611");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Borderlands 2.json");
            PresenceRuntime.Start(nameof(BL2), "Borderlands2", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("Borderlands2");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("Borderlands2"))
            {
                int level = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x016ACA88, [0x24, 0x258]), true);

                try
                {
                    if (level >= 1 && level <= 80)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Borderlands 2", placeholders);
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
                                LargeImageText = "Borderlands 2"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                }
                catch
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Borderlands 2"
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
            int money = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x016ACA88, [0x24, 0x2A0]), true);
            int eridium = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x016ACA88, [0x24, 0x2B4]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x016ACA88, [0x24, 0x258]), true);
            float hp = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x016AD42C, [0x0, 0x384, 0x6C]), true);
            float maxhp = Hypervisor.Read<float>(Hypervisor.GetPointer32(0x016AD42C, [0x0, 0x384, 0x5C]), true);

            int hp_rounded = (int)Math.Round(hp, 0, MidpointRounding.AwayFromZero);
            int maxhp_rounded = (int)Math.Round(maxhp, 0, MidpointRounding.AwayFromZero);

            return new Dictionary<string, object>
            {
                { "money", money },
                { "eridium", eridium },
                { "level", level },
                { "health", hp_rounded },
                { "maxhealth", maxhp_rounded }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}

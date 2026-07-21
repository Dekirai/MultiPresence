#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class BL1
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1489591356023115786");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Borderlands 1.json");
            PresenceRuntime.Start(nameof(BL1), "BorderlandsGOTY", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("BorderlandsGOTY");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("BorderlandsGOTY"))
            {
                int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x025EC5E0, [0x48, 0x32C]), true);

                try
                {
                    if (level >= 1 && level <= 69)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Borderlands 1", placeholders);
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
                                LargeImageText = "Borderlands GOTY Enhanced"
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
                            LargeImageText = "Borderlands GOTY Enhanced"
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
            int money = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x025EC5E0, [0x48, 0x350]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x025EC5E0, [0x48, 0x32C]), true);
            float hp = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x025EC5E0, [0x50, 0x288, 0x98]), true);
            float maxhp = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x025EC5E0, [0x50, 0x288, 0x80]), true);

            int hp_rounded = (int)Math.Round(hp, 0, MidpointRounding.AwayFromZero);
            int maxhp_rounded = (int)Math.Round(maxhp, 0, MidpointRounding.AwayFromZero);

            return new Dictionary<string, object>
            {
                { "money", money },
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

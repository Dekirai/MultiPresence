#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class SPTG
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1394219480871342223");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Scott Pilgrim vs The World.json");
            PresenceRuntime.Start(nameof(SPTG), "scott", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("scott");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("scott"))
            {
                try
                {
                    ulong _base = Hypervisor.GetPointer64(0x0230FF48, [0xC8, 0x1A8, 0x178]);
                    float health = Hypervisor.Read<float>(_base, true);

                    if (health > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Scott Pilgrim vs The World", placeholders);
                    }
                    else
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Menus",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Scott Pilgrim vs The World"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                }
                catch
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Menus",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Scott Pilgrim vs The World"
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
            ulong _base = Hypervisor.GetPointer64(0x0230FF48, [0xC8, 0x1A8, 0x178]);
            float health = Hypervisor.Read<float>(_base, true);
            float gutpoints = Hypervisor.Read<float>(_base + 0x1BC, true);
            float defense = Hypervisor.Read<float>(_base + 0x6F0, true);
            float speed = Hypervisor.Read<float>(_base + 0x6F4, true);
            float willpower = Hypervisor.Read<float>(_base + 0x6F8, true);
            float strength = Hypervisor.Read<float>(_base + 0x6FC, true);
            int lives = Hypervisor.Read<int>(_base + 0x700, true);
            float money = Hypervisor.Read<float>(_base + 0x1278, true);
            float totalexperience = Hypervisor.Read<float>(_base + 0x127C, true);

            int money_rounded = (int)Math.Round(money, 0, MidpointRounding.AwayFromZero);

            return new Dictionary<string, object>
            {
                { "health", health },
                { "gutpoints", gutpoints },
                { "defense", defense },
                { "speed", speed },
                { "willpower", willpower },
                { "strength", strength },
                { "lives", lives },
                { "money", money_rounded },
                { "totalexperience", totalexperience }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
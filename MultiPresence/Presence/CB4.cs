#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class CB4
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1395058700892377190");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Crash Bandicoot 4.json");
            PresenceRuntime.Start(nameof(CB4), "CrashBandicoot4", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("CrashBandicoot4");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("CrashBandicoot4"))
            {
                int destroyablecrates = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x158, 0x8, 0x430]), true);

                try
                {
                    if (destroyablecrates > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Crash Bandicoot 4: It's About Time", placeholders);
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
                                LargeImageText = "Crash Bandicoot 4: It's About Time"
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
                            LargeImageText = "Crash Bandicoot 4: It's About Time"
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
            int destroyablecrates = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x158, 0x8, 0x430]), true);
            int crates = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x158, 0x8, 0x438]), true);
            int deaths = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x158, 0x8, 0x5A8]), true);
            int wumpasretro = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x30, 0xA8, 0x70, 0x3F8]), true);
            int wumpasmodern = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x043F1A28, [0x30, 0xA8, 0x70, 0x408]), true);

            return new Dictionary<string, object>
            {
                { "maxcrates", destroyablecrates },
                { "currentcrates", crates },
                { "deaths", deaths },
                { "wumpasretro", wumpasretro },
                { "wumpasmodern", wumpasmodern }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}

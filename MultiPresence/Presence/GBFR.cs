#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class GBFR
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1426303485208559616");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Granblue Fantasy Relink.json");
            PresenceRuntime.Start(nameof(GBFR), "granblue_fantasy_relink", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("granblue_fantasy_relink");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("granblue_fantasy_relink"))
            {
                int _maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x05E59900, [0x164]), true);

                try
                {
                    if (_maxhealth > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Granblue Fantasy: Relink", placeholders);
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
                                LargeImageText = "Granblue Fantasy: Relink"
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
                            LargeImageText = "Granblue Fantasy: Relink"
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
            int _health = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x05E59900, [0x160]), true);
            int _maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x05E59900, [0x164]), true);
            int _level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x05FCA4F0, [0x140, 0x38, 0x3C]), true);
            int _money = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x05E48228, [0x30]), true);
            string _name = Hypervisor.ReadString(Hypervisor.GetPointer64(0x05E48228, [0x2B0]), 16, true);

            return new Dictionary<string, object>
            {
                { "health", _health },
                { "maxhealth", _maxhealth },
                { "level", _level },
                { "money", _money },
                { "name", _name }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
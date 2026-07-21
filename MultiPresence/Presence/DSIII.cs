#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DSIII
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1344011934206529618");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Dark Souls III.json");
            PresenceRuntime.Start(nameof(DSIII), "DarkSoulsIII", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("DarkSoulsIII");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("DarkSoulsIII"))
            {
                int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0x94]), true);

                try
                {
                    if (maxhp > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Dark Souls III", placeholders);
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
                                LargeImageText = "Dark Souls III"
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
                            LargeImageText = "Dark Souls III"
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
            string nickname = Hypervisor.ReadString(Hypervisor.GetPointer64(0x047572B8, [0x10, 0x88]), 20, true, null, true);
            int clearcount = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x047572B8, [0x78]), true);
            int archetype = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x016148F0, [0xA8, 0xC0, 0x64]), true);
            int deaths = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x047572B8, [0x98]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x047572B8, [0x10, 0x70]), true);
            int souls = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x047572B8, [0x10, 0x74]), true);
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0x90]), true);
            int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0x94]), true);
            int mp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0x9C]), true);
            int maxmp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0xA0]), true);
            int stamina = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0xA8]), true);
            int maxstamina = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04543F60, [0x28, 0x3A0, 0x70, 0xAC]), true);

            string classname = archetype switch
            {
                0 => "Knight",
                1 => "Mercenary",
                2 => "Warrior",
                3 => "Herald",
                4 => "Thief",
                5 => "Assassin",
                6 => "Sorcerer",
                7 => "Pyromancer",
                8 => "Cleric",
                9 => "Deprived",
                10 => "Debug",
                _ => "Unknown"
            };

            return new Dictionary<string, object>
            {
                { "nickname", nickname },
                { "clearcount", clearcount },
                { "class", classname },
                { "deaths", deaths },
                { "level", level },
                { "souls", souls },
                { "hp", hp },
                { "maxhp", maxhp },
                { "mp", mp },
                { "maxmp", maxmp },
                { "stamina", stamina },
                { "maxstamina", maxstamina }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}

#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DSII
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1344011274803220542");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Dark Souls II.json");
            PresenceRuntime.Start(nameof(DSII), "DarkSoulsII", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("DarkSoulsII");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("DarkSoulsII"))
            {
                int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x016148F0, [0xD0, 0x170]), true);

                try
                {
                    if (maxhp > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Dark Souls II", placeholders);
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
                                LargeImageText = "Dark Souls II"
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
                            LargeImageText = "Dark Souls II"
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
            string nickname = Hypervisor.ReadString(Hypervisor.GetPointer64(0x016148F0, [0xA8, 0xC0, 0x24]), 20, true, null, true);
            int clearcount = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x016148F0, [0xA8, 0xC0, 0x68]), true);
            int archetype = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x016148F0, [0xA8, 0xC0, 0x64]), true);
            int deaths = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x016148F0, [0xD0, 0x390, 0x294]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x016148F0, [0xD0, 0x390, 0x1C0]), true);
            int souls = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x016148F0, [0xD0, 0x390, 0x1E4]), true);
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x016148F0, [0xD0, 0x168]), true);
            int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x016148F0, [0xD0, 0x170]), true);
            float stamina = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x016148F0, [0xD0, 0x1AC]), true);
            float maxstamina = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x016148F0, [0xD0, 0x1B4]), true);

            string classname = archetype switch
            {
                1 => "Warrior",
                2 => "Knight",
                4 => "Bandit",
                5 => "Hunter",
                6 => "Cleric",
                7 => "Sorcerer",
                8 => "Explorer",
                9 => "Swordsman",
                10 => "Deprived",
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

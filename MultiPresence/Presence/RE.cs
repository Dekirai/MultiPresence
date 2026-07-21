#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.RE;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212466561068171294");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 1.json");
            PresenceRuntime.Start(nameof(RE), "bhd", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("bhd");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("bhd"))
            {
                try
                {
                    int floor_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x97C9C0, [0xE472C]), true);

                    if (floor_get > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil", placeholders);
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
                                LargeImageText = "Resident Evil"
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
                            LargeImageText = "Resident Evil"
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
            int floor_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x97C9C0, [0xE472C]), true);
            int stage_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x97C9C0, [0xE4730]), true);
            int character_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x97C9C0, [0x5118]), true);
            int health = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x9E41BC, [0x23C, 0x154, 0x2BC, 0x3BC]), true);
            int maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x9E41BC, [0x23C, 0x154, 0x2BC, 0x3C0]), true);
            int weapon_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x97C9C0, [0x80]), true);
            int ammoclip = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x9E41BC, [0x1C4, 0xFFC]), true);
            int maxammoclip = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x9E41BC, [0x1C4, 0x1000]), true);
            var stagevalue = await Stages.GetStage(floor_get);
            string character = "";
            string character_icon = "";

            string weapon = await Weapons.GetWeapon(weapon_get);

            string healthstatus = "";

            double percentage = (double)health / maxhealth * 100;

            if (percentage > 75)
                healthstatus = "Fine";
            else if (percentage > 50)
                healthstatus = "Caution";
            else if (percentage > 25)
                healthstatus = "Caution";
            else
                healthstatus = "Danger";

            string[] stage = stagevalue[stage_get].Split(':');

            if (character_get == 0)
            {
                character = "Chris Redfield";
                character_icon = "chris";
            }
            else if (character_get == 1)
            {
                character = "Jill Valentine";
                character_icon = "jill";
            }
            else if (character_get == 2)
            {
                character = "Rebecca Chambers";
                character_icon = "rebecca";
            }

            return new Dictionary<string, object>
            {
                { "floor", stage[0] },
                { "room", stage[1] },
                { "character", character },
                { "character_icon", character_icon },
                { "health", health },
                { "maxhealth", maxhealth },
                { "healthstatus", healthstatus },
                { "weapon", weapon },
                { "ammoclip", ammoclip },
                { "maxammoclip", maxammoclip }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
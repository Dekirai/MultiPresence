#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.RE2;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE2R
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1349630099510657079");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 2 Remake.json");
            PresenceRuntime.Start(nameof(RE2R), "re2", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("re2");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("re2"))
            {
                int maxhealth = Hypervisor.Read<short>(Hypervisor.GetPointer64(0x91A6CB0, [0x40, 0x624]), true);

                if (maxhealth > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 2", placeholders);
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
                            LargeImageText = "Resident Evil 2"
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
            int health = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x91A6CB0, [0x40, 0x628]), true);
            int maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x91A6CB0, [0x40, 0x624]), true);
            int weapon_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x91A8038, [0x188, 0x2D0]), true);
            int ammo = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x91A6DC0, [0x50, 0xA0, 0x18, 0x10, 0x20]), true);

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

            return new Dictionary<string, object>
            {
                { "health", health },
                { "maxhealth", maxhealth },
                { "healthstatus", healthstatus },
                { "healthpercentage", percentage },
                { "weapon", weapon },
                { "ammo", ammo },
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
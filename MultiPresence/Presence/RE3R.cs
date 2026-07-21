#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.RE3;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE3R
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1467486061952372879");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 3 Remake.json");
            PresenceRuntime.Start(nameof(RE3R), "re3", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("re3");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("re3"))
            {
                int maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x09A60970, [0x48, 0x234]), true);

                if (maxhealth > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 3", placeholders);
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
                            LargeImageText = "Resident Evil 3"
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
            int health = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x09A60970, [0x48, 0x238]), true);
            int maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x09A60970, [0x48, 0x234]), true);
            int weapon_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x09A6A350, [0x90, 0x190, 0x54]), true);
            int ammo = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x09A682A0, [0x50, 0xA0, 0x18, 0x10, 0x20]), true);

            string weapon = await Weapons.GetWeapon(weapon_get);

            string healthstatus = "";
            string healthstatusemoji = "";

            double percentage = (double)health / maxhealth * 100;

            if (percentage > 75)
                healthstatus = "Fine";
            else if (percentage > 50)
                healthstatus = "Caution";
            else if (percentage > 25)
                healthstatus = "Caution";
            else
                healthstatus = "Danger";

            if (percentage > 75)
                healthstatusemoji = "🟢";
            else if (percentage > 50)
                healthstatusemoji = "🟡";
            else if (percentage > 25)
                healthstatusemoji = "🟠";
            else
                healthstatusemoji = "🔴";

            return new Dictionary<string, object>
            {
                { "health", health },
                { "maxhealth", maxhealth },
                { "healthstatus", healthstatus },
                { "healthstatusemoji", healthstatusemoji },
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
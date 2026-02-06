using DiscordRPC;
using MultiPresence.Models.RE3;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE3R
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1467486061952372879");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 3 Remake.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("re3")[0];
                if (_myProcess.Id > 0)
                    Hypervisor.AttachProcess(_myProcess);
            }
            catch
            {
                //nothing?
            }
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName("re3");
            if (game.Length > 0)
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

                await Task.Delay(3000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                MainForm.gameUpdater.Start();
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
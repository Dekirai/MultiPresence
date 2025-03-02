using DiscordRPC;
using MultiPresence.Models.RE;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE
    {

        static string process = "bhd";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212466561068171294");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("bhd")[0];
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
            Process[] game = Process.GetProcessesByName("bhd");
            if (game.Length > 0)
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

                await Task.Delay(1000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int floor_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x97C9C0, [0xE472C]), true);
            int stage_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x97C9C0, [0xE4730]), true);
            int character_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x97C9C0, [0x5118]), true);
            int health = Hypervisor.Read<int>(0x2E5D470C, true);
            int weapon_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x97C9C0, [0x80]), true);
            int ammoclip = Hypervisor.Read<int>(0x2E5FF8BC, true);
            int maxammoclip = Hypervisor.Read<int>(0x2E5FF8C0, true);
            int maxhealth = Hypervisor.Read<int>(0x2E5D4710, true);
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
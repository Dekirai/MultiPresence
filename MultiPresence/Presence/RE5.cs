using DiscordRPC;
using MultiPresence.Models.RE5;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE5
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212083539567186002");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("re5dx9")[0];
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
            Process[] game = Process.GetProcessesByName("re5dx9");
            if (game.Length > 0)
            {
                int chris_health = Hypervisor.Read<short>(Hypervisor.GetPointer32(0x00DB27DC, [0x24, 0x1364]), true);
                int sheva_health = Hypervisor.Read<short>(Hypervisor.GetPointer32(0x00DB27DC, [0x28, 0x1364]), true);

                if (chris_health > 0 || sheva_health > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 5", placeholders);
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
                            LargeImageText = "Resident Evil 5"
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
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int stage_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0xDB2158, [0x273D8]), true);
            int money = Hypervisor.Read<int>(Hypervisor.GetPointer32(0xDB2158, [0x1C0]), true);
            int chris_health = Hypervisor.Read<short>(Hypervisor.GetPointer32(0xDB27DC, [0x24, 0x1364]), true);
            int sheva_health = Hypervisor.Read<short>(Hypervisor.GetPointer32(0xDB27DC, [0x28, 0x1364]), true);
            int item_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0xE340B0, [0x38C, 0x104]), true);
            int item_ammo = Hypervisor.Read<int>(Hypervisor.GetPointer32(0xE340B0, [0x38C, 0x108]), true);
            int item_maxammo = Hypervisor.Read<int>(Hypervisor.GetPointer32(0xE340B0, [0x38C, 0x118]), true);
            int item_pouch = Hypervisor.Read<int>(Hypervisor.GetPointer32(0xE340B0, [0x38C, 0x120]), true);
            var stagevalue = await Stages.GetStage(stage_get);

            string[] stage = stagevalue.Split(':');
            string item = await Weapons.GetWeapon(item_get);

            return new Dictionary<string, object>
            {
                { "chapter", stage[0] },
                { "room", stage[1] },
                { "money", money },
                { "item", item },
                { "ammo", item_ammo },
                { "maxammo", item_maxammo },
                { "pouch", item_pouch },
                { "chris_health", chris_health },
                { "sheva_health", sheva_health }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
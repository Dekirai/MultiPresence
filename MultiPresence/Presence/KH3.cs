using DiscordRPC;
using MultiPresence.Models.KH3;
using MultiPresence.Models.KHIII;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KH3
    {
        public static ulong _room_address = 0;
        static private DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static string difficulty = "";
        public static string? room = null;
        public static int level = 0;
        public static int gummilevel = 0;
        public static string[]? world = null;
        public static async void DoAction()
        {
            await Task.Delay(7500); // Wait 7.5 seconds to make sure that the AoB is actually findable
            GetPID();
            _room_address = (ulong)Hypervisor.FindSignature("?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2F 53 63 72 69 70 74 2F 54 72 65 73 47 61 6D 65 2E 54 72 65 73 50 6C 61 79 65 72 43 6F 6E 74 72 6F 6C 6C 65 72 53 6F 72 61");
            discord = new DiscordRpcClient("827190870724837406");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("KINGDOM HEARTS III")[0];
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
            Process[] game = Process.GetProcessesByName("KINGDOM HEARTS III");
            if (game.Length > 0)
            {
                try
                {
                    string level_path = Hypervisor.ReadString(_room_address, 27, true);
                    world = await Worlds.GetWorld(level_path.Split('/')[3]);

                    if (world[0] == "Main Menu")
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Main Menu",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Kingdom Hearts III"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                    else
                    {
                        if (level_path.Contains("wm"))
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts III", placeholders, "World_Map");
                        }
                        else if (level_path.Contains("gm"))
                        {
                            gummilevel = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x09D8E920, [0x48, 0x470, 0x550, 0x250, 0xD0, 0x228, 0x16C]), true);

                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts III", placeholders, "Gummi_Ship");
                        }
                        else
                        {
                            level = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x09D8E920, [0x48, 0x458, 0x188, 0x1B8, 0x4D0, 0x40]), true);

                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts III", placeholders, "In_World");
                        }
                    }
                }
                catch
                {
                    discord.UpdateLargeAsset("logo", "Kingdom Hearts III");
                    discord.UpdateDetails($"In Main Menu");
                    discord.UpdateState("");
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
            string level_path = Hypervisor.ReadString(_room_address, 27, true);
            int difficulty_get = Hypervisor.Read<byte>(0x87ECC8C);

            room = await Rooms.GetRoom(level_path.Split('/')[4]);
            world = await Worlds.GetWorld(level_path.Split('/')[3]);
            difficulty = await Difficulties.GetDifficulty(difficulty_get);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "gummilevel", gummilevel },
                { "room", room },
                { "world", world[0] },
                { "world_icon_name", world[1] },
                { "difficulty", difficulty }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}

using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class TBOI
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static bool isScanned = false;
        public static ulong _main_address = 0;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1378819491902918758");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/The Binding of Isaac Rebirth.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("isaac-ng")[0];
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
            Process[] game = Process.GetProcessesByName("isaac-ng");
            if (game.Length > 0)
            {
                int floornumber = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x7E49F4, [0x0]), true);

                try
                {
                    if (floornumber > 0)
                    {
                        if (!isScanned)
                        {
                            _main_address = (ulong)Hypervisor.FindSignature("48 65 61 64 44 6F 77 6E 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 ?? ?? ?? 00 ?? 00 ?? 00 00 00 ?? ?? ?? ?? ??");
                            isScanned = true;
                        }
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "The Binding of Isaac: Rebirth", placeholders);
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
                                LargeImageText = "The Binding of Isaac: Rebirth"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                        isScanned = false;
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
                            LargeImageText = "The Binding of Isaac: Rebirth"
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
            int floornumber = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x7E49F4, [0x0]), true);
            int floortype = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x7E49F4, [0x4]), true);
            int ticks = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x7E49F4, [0x25A70]), true);
            int score = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x7E49F4, [0x25C90]), true);
            string character = Hypervisor.ReadString(_main_address + 0xA0, 32, true);
            string characterasset = $"char_{character.ToLower().Replace(" ", "")}";

            if (character == "Esau")
                character = "Jacob & Esau";
            else if (character == "The Soul")
                character = "The Forgotten";

            //--- INGAME STATS ---//
            int maxredhearts = Hypervisor.Read<byte>(_main_address + 0x20, true);
            int redhearts = Hypervisor.Read<byte>(_main_address + 0x24, true);
            int whitehearts = Hypervisor.Read<byte>(_main_address + 0x28, true);
            int bluehearts = Hypervisor.Read<byte>(_main_address + 0x2C, true);
            int blackheartsmask = Hypervisor.Read<byte>(_main_address + 0x30, true);
            int bonehearts = Hypervisor.Read<byte>(_main_address + 0x54, true);
            int boneheartsmask = Hypervisor.Read<byte>(_main_address + 0x58, true);
            int rottenhearts = Hypervisor.Read<byte>(_main_address + 0x70, true);
            int goldenhearts = Hypervisor.Read<byte>(_main_address + 0x618, true);
            //--- INGAME STATS END ---//

            //--- GLOBAL STATS ---//
            int winstreak = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x007E4A1C, [0x328]), true);
            int edentokens = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x007E4A1C, [0x324]), true);
            //--- GLOBAL STATS END ---//

            //--- COLLECTIBLES ---//
            int keys = Hypervisor.Read<byte>(_main_address + 0x3C, true);
            int bombs = Hypervisor.Read<byte>(_main_address + 0x44, true);
            int coins = Hypervisor.Read<short>(_main_address + 0x48, true);
            //--- COLLECTIBLES END ---//

            int totalSeconds = ticks / 30;

            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            string time = $"{hours:D2}:{minutes:D2}:{seconds:D2}";


            string floorlevel = floornumber switch
            {
                1 => "Basement / Cellar I",
                2 => "Basement / Cellar II",
                3 => "Caves I",
                4 => "Caves II",
                5 => "Depths I",
                6 => "Depths II",
                7 => "Womb I",
                8 => "Womb II",
                10 => "Cathedral / Dark Room",
                11 => "Chest",
                12 => "Void",
                13 => "Home",
                _ => ""
            };

            string floortypename = floortype switch
            {
                0 => "Basement",
                1 => "Cellar",
                2 => "Burning Basement",
                3 => "Caves",
                4 => "Downpour",
                5 => "Dross",
                6 => "Depths",
                7 => "Necropolis",
                8 => "Dank Depths",
                9 => "Womb",
                10 => "Utero",
                11 => "Scarred Womb",
                12 => "Hush",
                13 => "Sheol",
                14 => "Cathedral",
                15 => "Dark Room",
                16 => "Chest",
                30 => "Mausoleum",
                31 => "Gehenna",
                32 => "Mother",
                34 => "Home",
                _ => "Unknown Floor Type"
            };

            string floortypeasset;

            if (floortype == 0 && floorlevel.Contains("Basement"))
                floortypeasset = "floor_basement";
            else if (floortype == 0 && floorlevel.Contains("Caves"))
                floortypeasset = "floor_caves";
            else if (floortype == 0 && floorlevel.Contains("Basement"))
                floortypeasset = "floor_depths";
            else if (floortype == 0 && floorlevel.Contains("Womb"))
                floortypeasset = "floor_womb";
            else if (floortype == 0 && floorlevel.Contains("Cathedral"))
                floortypeasset = "floor_cathedral";
            else if (floortype == 0 && floorlevel.Contains("Chest"))
                floortypeasset = "floor_chest";
            else if (floortype == 0 && floorlevel.Contains("Void"))
                floortypeasset = "floor_void";
            else if (floortype == 0 && floorlevel.Contains("Home"))
                floortypeasset = "floor_home";
            else floortypeasset = await Models.TBOI.FloorAssets.GetFloorAsset(floortype);

            return new Dictionary<string, object>
            {
                { "floor", floorlevel },
                { "floortype", floortypename },
                { "floortypeasset", floortypeasset },
                { "time", time },
                { "score", score },
                { "character", character },
                { "characterasset", characterasset },
                { "keys", keys },
                { "bombs", bombs },
                { "coins", coins },
                { "maxredhearts", maxredhearts },
                { "redhearts", redhearts },
                { "whitehearts", whitehearts },
                { "bluehearts", bluehearts },
                { "blackheartsmask", blackheartsmask },
                { "bonehearts", bonehearts },
                { "boneheartsmask", boneheartsmask },
                { "rottenhearts", rottenhearts },
                { "goldenhearts", goldenhearts },
                { "winstreak", winstreak },
                { "edentokens", edentokens }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}

using DiscordRPC;
using MultiPresence.Models.KH3;
using MultiPresence.Models.KHIII;
using MultiPresence.Properties;
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
                    int difficulty_get = Hypervisor.Read<byte>(0x87ECC8C);

                    if (Settings.Default.langDE == true)
                    {
                        room = await Rooms.GetRoomDE(level_path.Split('/')[4]);
                        world = await Worlds.GetWorldDE(level_path.Split('/')[3]);
                        difficulty = await Difficulties.GetDifficultyDE(difficulty_get);
                    }
                    else
                    {
                        room = await Rooms.GetRoom(level_path.Split('/')[4]);
                        world = await Worlds.GetWorld(level_path.Split('/')[3]);
                        difficulty = await Difficulties.GetDifficulty(difficulty_get);
                    }

                    var placeholders = new Dictionary<string, object>
                    {
                        { "level", level },
                        { "gummilevel", gummilevel },
                        { "room", room },
                        { "world", world[0] },
                        { "world_icon_name", world[1] },
                        { "difficulty", difficulty }
                    };

                    if (world[0] == "Main Menu")
                    {
                        discord.UpdateLargeAsset("logo", "Kingdom Hearts III");
                        discord.UpdateDetails($"In Main Menu");
                        discord.UpdateState("");
                    }
                    else
                    {
                        if (level_path.Contains("wm"))
                        {
                            string details = updater.UpdateDetails("Kingdom Hearts III", placeholders, "World_Map");
                            string state = updater.UpdateState("Kingdom Hearts III", placeholders, "World_Map");
                            string largeasset = updater.UpdateLargeAsset("Kingdom Hearts III", placeholders, "World_Map");
                            string largeassettext = updater.UpdateLargeAssetText("Kingdom Hearts III", placeholders, "World_Map");
                            string smallasset = updater.UpdateSmallAsset("Kingdom Hearts III", placeholders, "World_Map");
                            string smallassettext = updater.UpdateSmallAssetText("Kingdom Hearts III", placeholders, "World_Map");
                            string button1text = updater.UpdateButton1Text("Kingdom Hearts III", placeholders, "World_Map");
                            string button2text = updater.UpdateButton2Text("Kingdom Hearts III", placeholders, "World_Map");
                            string button1url = updater.UpdateButton1URL("Kingdom Hearts III", placeholders, "World_Map");
                            string button2url = updater.UpdateButton2URL("Kingdom Hearts III", placeholders, "World_Map");
                            discord.UpdateLargeAsset(largeasset, largeassettext);
                            discord.UpdateSmallAsset(smallasset, smallassettext);
                            discord.UpdateDetails(details);
                            discord.UpdateState(state);

                            if (button1url.Length > 0 && button2url.Length == 0)
                            {
                                discord.UpdateButtons(new DiscordRPC.Button[]
                                {
        new DiscordRPC.Button() { Label = button1text, Url = button1url }
                                });
                            }
                            else if (button1url.Length > 0 && button2url.Length > 0)
                            {
                                discord.UpdateButtons(new DiscordRPC.Button[]
                                {
        new DiscordRPC.Button() { Label = button1text, Url = button1url },
        new DiscordRPC.Button() { Label = button2text, Url = button2url }
                                });
                            }
                            else
                            {
                                discord.UpdateButtons(null);
                            }
                        }
                        else if (level_path.Contains("gm"))
                        {
                            gummilevel = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x09D8E920, [0x48, 0x470, 0x550, 0x250, 0xD0, 0x228, 0x16C]), true);

                            string details = updater.UpdateDetails("Kingdom Hearts III", placeholders, "Gummi_Ship");
                            string state = updater.UpdateState("Kingdom Hearts III", placeholders, "Gummi_Ship");
                            string largeasset = updater.UpdateLargeAsset("Kingdom Hearts III", placeholders, "Gummi_Ship");
                            string largeassettext = updater.UpdateLargeAssetText("Kingdom Hearts III", placeholders, "Gummi_Ship");
                            string smallasset = updater.UpdateSmallAsset("Kingdom Hearts III", placeholders, "Gummi_Ship");
                            string smallassettext = updater.UpdateSmallAssetText("Kingdom Hearts III", placeholders, "Gummi_Ship");
                            string button1text = updater.UpdateButton1Text("Kingdom Hearts III", placeholders, "Gummi_Ship");
                            string button2text = updater.UpdateButton2Text("Kingdom Hearts III", placeholders, "Gummi_Ship");
                            string button1url = updater.UpdateButton1URL("Kingdom Hearts III", placeholders, "Gummi_Ship");
                            string button2url = updater.UpdateButton2URL("Kingdom Hearts III", placeholders, "Gummi_Ship");
                            discord.UpdateLargeAsset(largeasset, largeassettext);
                            discord.UpdateSmallAsset(smallasset, smallassettext);
                            discord.UpdateDetails(details);
                            discord.UpdateState(state);

                            if (button1url.Length > 0 && button2url.Length == 0)
                            {
                                discord.UpdateButtons(new DiscordRPC.Button[]
                                {
        new DiscordRPC.Button() { Label = button1text, Url = button1url }
                                });
                            }
                            else if (button1url.Length > 0 && button2url.Length > 0)
                            {
                                discord.UpdateButtons(new DiscordRPC.Button[]
                                {
        new DiscordRPC.Button() { Label = button1text, Url = button1url },
        new DiscordRPC.Button() { Label = button2text, Url = button2url }
                                });
                            }
                            else
                            {
                                discord.UpdateButtons(null);
                            }
                        }
                        else
                        {
                            level = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x09D8E920, [0x48, 0x458, 0x188, 0x1B8, 0x4D0, 0x40]), true);

                            string details = updater.UpdateDetails("Kingdom Hearts III", placeholders, "In_World");
                            string state = updater.UpdateState("Kingdom Hearts III", placeholders, "In_World");
                            string largeasset = updater.UpdateLargeAsset("Kingdom Hearts III", placeholders, "In_World");
                            string largeassettext = updater.UpdateLargeAssetText("Kingdom Hearts III", placeholders, "In_World");
                            string smallasset = updater.UpdateSmallAsset("Kingdom Hearts III", placeholders, "In_World");
                            string smallassettext = updater.UpdateSmallAssetText("Kingdom Hearts III", placeholders, "In_World");
                            string button1text = updater.UpdateButton1Text("Kingdom Hearts III", placeholders, "In_World");
                            string button2text = updater.UpdateButton2Text("Kingdom Hearts III", placeholders, "In_World");
                            string button1url = updater.UpdateButton1URL("Kingdom Hearts III", placeholders, "In_World");
                            string button2url = updater.UpdateButton2URL("Kingdom Hearts III", placeholders, "In_World");
                            discord.UpdateLargeAsset(largeasset, largeassettext);
                            discord.UpdateSmallAsset(smallasset, smallassettext);
                            discord.UpdateDetails(details);
                            discord.UpdateState(state);

                            if (button1url.Length > 0 && button2url.Length == 0)
                            {
                                discord.UpdateButtons(new DiscordRPC.Button[]
                                {
        new DiscordRPC.Button() { Label = button1text, Url = button1url }
                                });
                            }
                            else if (button1url.Length > 0 && button2url.Length > 0)
                            {
                                discord.UpdateButtons(new DiscordRPC.Button[]
                                {
        new DiscordRPC.Button() { Label = button1text, Url = button1url },
        new DiscordRPC.Button() { Label = button2text, Url = button2url }
                                });
                            }
                            else
                            {
                                discord.UpdateButtons(null);
                            }
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

        private static void InitializeDiscord()
        {
            discord.Initialize();
            discord.SetPresence(new RichPresence()
            {
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}

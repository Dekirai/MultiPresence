using DiscordRPC;
using Memory;
using MultiPresence.Models.MMBN6;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMBN6F
    {
        static Mem mem = new Mem();
        static string process = "MMBN_LC2";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1260222104444473405");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            int pid = mem.GetProcIdFromName(process);
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                int _game = mem.ReadByte("MMBN_LC2.exe+ABEF0A0");
                if (_game != 10)
                {
                    discord.Deinitialize();
                    MainForm.gameUpdater.Start();
                }
                else
                {
                    int area_get = mem.ReadByte("80205944");
                    int room_get = mem.ReadByte("80205945");
                    int hp = mem.Read2Byte("8020858C");
                    int maxhp = mem.Read2Byte("8020858E");
                    int hp_battle = mem.Read2Byte("8020A8F4");
                    int maxhp_battle = mem.Read2Byte("8020A8F6");
                    int gamestate = mem.ReadByte("80205940");
                    var location = await Areas.GetArea(area_get);

                    var placeholders = new Dictionary<string, object>
                    {
                        { "hp", hp },
                        { "hp_battle", hp_battle },
                        { "maxhp", maxhp },
                        { "maxhp_battle", maxhp_battle },
                        { "location", location[room_get] }
                    };

                    discord.UpdateLargeAsset($"logo", $"Mega Man Battle Network 6: Cybeast Falzar");
                    if (gamestate == 12)
                    {
                        string details = updater.UpdateDetails("Mega Man Battle Network 6", placeholders, "In_Battle");
                        string state = updater.UpdateState("Mega Man Battle Network 6", placeholders, "In_Battle");
                        string largeasset = updater.UpdateLargeAsset("Mega Man Battle Network 6", placeholders, "In_Battle");
                        string largeassettext = updater.UpdateLargeAssetText("Mega Man Battle Network 6", placeholders, "In_Battle");
                        string smallasset = updater.UpdateSmallAsset("Mega Man Battle Network 6", placeholders, "In_Battle");
                        string smallassettext = updater.UpdateSmallAssetText("Mega Man Battle Network 6", placeholders, "In_Battle");
                        string button1text = updater.UpdateButton1Text("Mega Man Battle Network 6", placeholders, "In_Battle");
                        string button2text = updater.UpdateButton2Text("Mega Man Battle Network 6", placeholders, "In_Battle");
                        string button1url = updater.UpdateButton1URL("Mega Man Battle Network 6", placeholders, "In_Battle");
                        string button2url = updater.UpdateButton2URL("Mega Man Battle Network 6", placeholders, "In_Battle");
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
                        string details = updater.UpdateDetails("Mega Man Battle Network 6", placeholders);
                        string state = updater.UpdateState("Mega Man Battle Network 6", placeholders);
                        string largeasset = updater.UpdateLargeAsset("Mega Man Battle Network 6", placeholders);
                        string largeassettext = updater.UpdateLargeAssetText("Mega Man Battle Network 6", placeholders);
                        string smallasset = updater.UpdateSmallAsset("Mega Man Battle Network 6", placeholders);
                        string smallassettext = updater.UpdateSmallAssetText("Mega Man Battle Network 6", placeholders);
                        string button1text = updater.UpdateButton1Text("Mega Man Battle Network 6", placeholders);
                        string button2text = updater.UpdateButton2Text("Mega Man Battle Network 6", placeholders);
                        string button1url = updater.UpdateButton1URL("Mega Man Battle Network 6", placeholders);
                        string button2url = updater.UpdateButton2URL("Mega Man Battle Network 6", placeholders);
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
                    await Task.Delay(3000);
                    Thread thread = new Thread(RPC);
                    thread.Start();
                }
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

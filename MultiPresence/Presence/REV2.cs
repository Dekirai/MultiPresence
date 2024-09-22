using DiscordRPC;
using Memory;
using MultiPresence.Models.REV2;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class REV2
    {
        static Mem mem = new Mem();
        static string process = "rerev2";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        static int mission = 0;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1213180163446149121");
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
                int raid_character_get = mem.ReadByte("rerev2.exe+117ED54,0x4A58");
                int raid_chapter_get = mem.ReadByte("rerev2.exe+117ED54,0x30");
                int raid_character_level = mem.ReadByte("rerev2.exe+117ED54,0x4A59");
                int raid_money = mem.ReadInt("rerev2.exe+117D120,0xBA08");
                int stage_get = mem.Read2Byte("rerev2.exe+115AACC");

                var stage = await Stages.GetStage(stage_get);
                var chapter = await Chapters.GetChapter(raid_chapter_get);

                var raid_character = await Characters.GetCharacter(raid_character_get);

                var placeholders = new Dictionary<string, object>
                    {
                        { "character", raid_character },
                        { "level", raid_character_level },
                        { "money", raid_money },
                        { "chapter", chapter },
                        { "mission", mission }
                    };

                if (stage[0] == "Raid Mode")
                {
                    if (stage[1] == "In Lobby")
                    {
                        mission = mem.ReadByte("rerev2.exe+011DE690,0x1E0,0x4C,0x3C,0x14,0x3C,0x74,0x7C4") + 1;
                        string details = updater.UpdateDetails("Resident Evil Revelations 2", placeholders, "Lobby");
                        string state = updater.UpdateState("Resident Evil Revelations 2", placeholders, "Lobby");
                        string largeasset = updater.UpdateLargeAsset("Resident Evil Revelations 2", placeholders, "Lobby");
                        string largeassettext = updater.UpdateLargeAssetText("Resident Evil Revelations 2", placeholders, "Lobby");
                        string smallasset = updater.UpdateSmallAsset("Resident Evil Revelations 2", placeholders, "Lobby");
                        string smallassettext = updater.UpdateSmallAssetText("Resident Evil Revelations 2", placeholders, "Lobby");
                        string button1text = updater.UpdateButton1Text("Resident Evil Revelations 2", placeholders, "Lobby");
                        string button2text = updater.UpdateButton2Text("Resident Evil Revelations 2", placeholders, "Lobby");
                        string button1url = updater.UpdateButton1URL("Resident Evil Revelations 2", placeholders, "Lobby");
                        string button2url = updater.UpdateButton2URL("Resident Evil Revelations 2", placeholders, "Lobby");
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
                        string details = updater.UpdateDetails("Resident Evil Revelations 2", placeholders, "Ingame");
                        string state = updater.UpdateState("Resident Evil Revelations 2", placeholders, "Ingame");
                        string largeasset = updater.UpdateLargeAsset("Resident Evil Revelations 2", placeholders, "Ingame");
                        string largeassettext = updater.UpdateLargeAssetText("Resident Evil Revelations 2", placeholders, "Ingame");
                        string smallasset = updater.UpdateSmallAsset("Resident Evil Revelations 2", placeholders, "Ingame");
                        string smallassettext = updater.UpdateSmallAssetText("Resident Evil Revelations 2", placeholders, "Ingame");
                        string button1text = updater.UpdateButton1Text("Resident Evil Revelations 2", placeholders, "Ingame");
                        string button2text = updater.UpdateButton2Text("Resident Evil Revelations 2", placeholders, "Ingame");
                        string button1url = updater.UpdateButton1URL("Resident Evil Revelations 2", placeholders, "Ingame");
                        string button2url = updater.UpdateButton2URL("Resident Evil Revelations 2", placeholders, "Ingame");
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
                else
                {
                    discord.UpdateLargeAsset($"logo", $"Resident Evil Revelations 2");
                    discord.UpdateDetails($"");
                    discord.UpdateState($"");
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
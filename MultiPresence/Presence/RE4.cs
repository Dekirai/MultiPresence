using DiscordRPC;
using MultiPresence.Models.RE4;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    //public class RE4
    //{

    //    static string process = "bio4";
    //    public static string _main_address = "";
    //    private static DiscordRpcClient? discord;
    //    private static DiscordStatusUpdater? updater;
    //    public static async void DoAction()
    //    {
    //        await Task.Delay(7500);
    //        GetPID();
    //        long main_get = (await mem.AoBScan("00 00 00 00 60 BB ?? ?? ?? ?? 96 40 00 00 00 00 E4 CB 96 40 ?? ?? ?? ?? E4 CB 96 40 00 00 00 00 E4 CB 96 40", true)).FirstOrDefault();
    //        _main_address = main_get.ToString("X8");
    //        discord = new DiscordRpcClient("982193093388427314");
    //        InitializeDiscord();
    //        updater = new DiscordStatusUpdater("config.json");
    //        Thread thread = new Thread(RPC);
    //        thread.Start();
    //    }

    //    private static void GetPID()
    //    {
    //        int pid = mem.GetProcIdFromName(process);
    //        bool openProc = false;

    //        if (pid > 0) openProc = mem.OpenProcess(pid);
    //    }

    //    private static async void RPC()
    //    {
    //        Process[] game = Process.GetProcessesByName(process);
    //        if (game.Length > 0)
    //        {
    //            string area = "";
    //            int stage = mem.ReadByte($"{_main_address}+5B85");
    //            int chapter = mem.ReadByte($"{_main_address}+5B72");
    //            int room = mem.ReadByte($"{_main_address}+5B84");
    //            int weapon = mem.ReadByte($"{_main_address}+1745C");
    //            int character = mem.ReadByte($"{_main_address}+5BA0");
    //            int score = mem.ReadInt($"{_main_address}+17544");
    //            int difficulty = mem.ReadByte($"{_main_address}+9054");

    //            var room_name = await Stages.GetStage(stage);
    //            var weapon_name = await Weapons.GetWeapon(weapon);
    //            var difficulty_name = await Difficulties.GetDifficulty(difficulty);
    //            var character_name = await Characters.GetCharacter(character);
    //            var chapter_name = await Chapters.GetChapter(chapter);

    //            if (stage == 1)
    //                area = "Village";
    //            if (stage == 2)
    //                area = "Castle";
    //            if (stage == 3)
    //                area = "Island";

    //            var placeholders = new Dictionary<string, object>
    //                {
    //                    { "room", room_name[room] },
    //                    { "chapter", chapter_name },
    //                    { "area", area },
    //                    { "area_icon_name", area.ToLower() },
    //                    { "difficulty", difficulty_name },
    //                    { "weapon", weapon_name },
    //                    { "character", character_name },
    //                    { "score", score},
    //                };

    //            if (stage == 1 && room == 32)
    //            {
    //                string details = updater.UpdateDetails("Resident Evil 4 (2005)", placeholders, "Title_Screen");
    //                string state = updater.UpdateState("Resident Evil 4 (2005)", placeholders, "Title_Screen");
    //                string largeasset = updater.UpdateLargeAsset("Resident Evil 4 (2005)", placeholders, "Title_Screen");
    //                string largeassettext = updater.UpdateLargeAssetText("Resident Evil 4 (2005)", placeholders, "Title_Screen");
    //                string smallasset = updater.UpdateSmallAsset("Resident Evil 4 (2005)", placeholders, "Title_Screen");
    //                string smallassettext = updater.UpdateSmallAssetText("Resident Evil 4 (2005)", placeholders, "Title_Screen");
    //                string button1text = updater.UpdateButton1Text("Resident Evil 4 (2005)", placeholders, "Title_Screen");
    //                string button2text = updater.UpdateButton2Text("Resident Evil 4 (2005)", placeholders, "Title_Screen");
    //                string button1url = updater.UpdateButton1URL("Resident Evil 4 (2005)", placeholders, "Title_Screen");
    //                string button2url = updater.UpdateButton2URL("Resident Evil 4 (2005)", placeholders, "Title_Screen");
    //                discord.UpdateLargeAsset(largeasset, largeassettext);
    //                discord.UpdateSmallAsset(smallasset, smallassettext);
    //                discord.UpdateDetails(details);
    //                discord.UpdateState(state);

    //                if (button1url.Length > 0 && button2url.Length == 0)
    //                {
    //                    discord.UpdateButtons(new DiscordRPC.Button[]
    //                    {
    //                            new DiscordRPC.Button() { Label = button1text, Url = button1url }
    //                    });
    //                }
    //                else if (button1url.Length > 0 && button2url.Length > 0)
    //                {
    //                    discord.UpdateButtons(new DiscordRPC.Button[]
    //                    {
    //                            new DiscordRPC.Button() { Label = button1text, Url = button1url },
    //                            new DiscordRPC.Button() { Label = button2text, Url = button2url }
    //                    });
    //                }
    //                else
    //                {
    //                    discord.UpdateButtons(null);
    //                }
    //            }
    //            else
    //            {
    //                if (stage == 4)
    //                {
    //                    if (room >= 0 && room <= 4)
    //                    {
    //                        string details = updater.UpdateDetails("Resident Evil 4 (2005)", placeholders, "Mercenaries");
    //                        string state = updater.UpdateState("Resident Evil 4 (2005)", placeholders, "Mercenaries");
    //                        string largeasset = updater.UpdateLargeAsset("Resident Evil 4 (2005)", placeholders, "Mercenaries");
    //                        string largeassettext = updater.UpdateLargeAssetText("Resident Evil 4 (2005)", placeholders, "Mercenaries");
    //                        string smallasset = updater.UpdateSmallAsset("Resident Evil 4 (2005)", placeholders, "Mercenaries");
    //                        string smallassettext = updater.UpdateSmallAssetText("Resident Evil 4 (2005)", placeholders, "Mercenaries");
    //                        string button1text = updater.UpdateButton1Text("Resident Evil 4 (2005)", placeholders, "Mercenaries");
    //                        string button2text = updater.UpdateButton2Text("Resident Evil 4 (2005)", placeholders, "Mercenaries");
    //                        string button1url = updater.UpdateButton1URL("Resident Evil 4 (2005)", placeholders, "Mercenaries");
    //                        string button2url = updater.UpdateButton2URL("Resident Evil 4 (2005)", placeholders, "Mercenaries");
    //                        discord.UpdateLargeAsset(largeasset, largeassettext);
    //                        discord.UpdateSmallAsset(smallasset, smallassettext);
    //                        discord.UpdateDetails(details);
    //                        discord.UpdateState(state);

    //                        if (button1url.Length > 0 && button2url.Length == 0)
    //                        {
    //                            discord.UpdateButtons(new DiscordRPC.Button[]
    //                            {
    //                            new DiscordRPC.Button() { Label = button1text, Url = button1url }
    //                            });
    //                        }
    //                        else if (button1url.Length > 0 && button2url.Length > 0)
    //                        {
    //                            discord.UpdateButtons(new DiscordRPC.Button[]
    //                            {
    //                            new DiscordRPC.Button() { Label = button1text, Url = button1url },
    //                            new DiscordRPC.Button() { Label = button2text, Url = button2url }
    //                            });
    //                        }
    //                        else
    //                        {
    //                            discord.UpdateButtons(null);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        string details = updater.UpdateDetails("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
    //                        string state = updater.UpdateState("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
    //                        string largeasset = updater.UpdateLargeAsset("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
    //                        string largeassettext = updater.UpdateLargeAssetText("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
    //                        string smallasset = updater.UpdateSmallAsset("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
    //                        string smallassettext = updater.UpdateSmallAssetText("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
    //                        string button1text = updater.UpdateButton1Text("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
    //                        string button2text = updater.UpdateButton2Text("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
    //                        string button1url = updater.UpdateButton1URL("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
    //                        string button2url = updater.UpdateButton2URL("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
    //                        discord.UpdateLargeAsset(largeasset, largeassettext);
    //                        discord.UpdateSmallAsset(smallasset, smallassettext);
    //                        discord.UpdateDetails(details);
    //                        discord.UpdateState(state);

    //                        if (button1url.Length > 0 && button2url.Length == 0)
    //                        {
    //                            discord.UpdateButtons(new DiscordRPC.Button[]
    //                            {
    //                            new DiscordRPC.Button() { Label = button1text, Url = button1url }
    //                            });
    //                        }
    //                        else if (button1url.Length > 0 && button2url.Length > 0)
    //                        {
    //                            discord.UpdateButtons(new DiscordRPC.Button[]
    //                            {
    //                            new DiscordRPC.Button() { Label = button1text, Url = button1url },
    //                            new DiscordRPC.Button() { Label = button2text, Url = button2url }
    //                            });
    //                        }
    //                        else
    //                        {
    //                            discord.UpdateButtons(null);
    //                        }
    //                    }
    //                }
    //                else if (stage == 5)
    //                {
    //                    string details = updater.UpdateDetails("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
    //                    string state = updater.UpdateState("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
    //                    string largeasset = updater.UpdateLargeAsset("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
    //                    string largeassettext = updater.UpdateLargeAssetText("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
    //                    string smallasset = updater.UpdateSmallAsset("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
    //                    string smallassettext = updater.UpdateSmallAssetText("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
    //                    string button1text = updater.UpdateButton1Text("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
    //                    string button2text = updater.UpdateButton2Text("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
    //                    string button1url = updater.UpdateButton1URL("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
    //                    string button2url = updater.UpdateButton2URL("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
    //                    discord.UpdateLargeAsset(largeasset, largeassettext);
    //                    discord.UpdateSmallAsset(smallasset, smallassettext);
    //                    discord.UpdateDetails(details);
    //                    discord.UpdateState(state);

    //                    if (button1url.Length > 0 && button2url.Length == 0)
    //                    {
    //                        discord.UpdateButtons(new DiscordRPC.Button[]
    //                        {
    //                            new DiscordRPC.Button() { Label = button1text, Url = button1url }
    //                        });
    //                    }
    //                    else if (button1url.Length > 0 && button2url.Length > 0)
    //                    {
    //                        discord.UpdateButtons(new DiscordRPC.Button[]
    //                        {
    //                            new DiscordRPC.Button() { Label = button1text, Url = button1url },
    //                            new DiscordRPC.Button() { Label = button2text, Url = button2url }
    //                        });
    //                    }
    //                    else
    //                    {
    //                        discord.UpdateButtons(null);
    //                    }
    //                }
    //                else
    //                {
    //                    string details = updater.UpdateDetails("Resident Evil 4 (2005)", placeholders);
    //                    string state = updater.UpdateState("Resident Evil 4 (2005)", placeholders);
    //                    string largeasset = updater.UpdateLargeAsset("Resident Evil 4 (2005)", placeholders);
    //                    string largeassettext = updater.UpdateLargeAssetText("Resident Evil 4 (2005)", placeholders);
    //                    string smallasset = updater.UpdateSmallAsset("Resident Evil 4 (2005)", placeholders);
    //                    string smallassettext = updater.UpdateSmallAssetText("Resident Evil 4 (2005)", placeholders);
    //                    string button1text = updater.UpdateButton1Text("Resident Evil 4 (2005)", placeholders);
    //                    string button2text = updater.UpdateButton2Text("Resident Evil 4 (2005)", placeholders);
    //                    string button1url = updater.UpdateButton1URL("Resident Evil 4 (2005)", placeholders);
    //                    string button2url = updater.UpdateButton2URL("Resident Evil 4 (2005)", placeholders);
    //                    discord.UpdateLargeAsset(largeasset, largeassettext);
    //                    discord.UpdateSmallAsset(smallasset, smallassettext);
    //                    discord.UpdateDetails(details);
    //                    discord.UpdateState(state);

    //                    if (button1url.Length > 0 && button2url.Length == 0)
    //                    {
    //                        discord.UpdateButtons(new DiscordRPC.Button[]
    //                        {
    //                            new DiscordRPC.Button() { Label = button1text, Url = button1url }
    //                        });
    //                    }
    //                    else if (button1url.Length > 0 && button2url.Length > 0)
    //                    {
    //                        discord.UpdateButtons(new DiscordRPC.Button[]
    //                        {
    //                            new DiscordRPC.Button() { Label = button1text, Url = button1url },
    //                            new DiscordRPC.Button() { Label = button2text, Url = button2url }
    //                        });
    //                    }
    //                    else
    //                    {
    //                        discord.UpdateButtons(null);
    //                    }
    //                }
    //            }

    //            await Task.Delay(3000);
    //            Thread thread = new Thread(RPC);
    //            thread.Start();
    //        }
    //        else
    //        {
    //            discord.Deinitialize();
    //            MainForm.gameUpdater.Start();
    //        }
    //    }

    //    private static void InitializeDiscord()
    //    {
    //        discord.Initialize();
    //        discord.SetPresence(new RichPresence()
    //        {
    //            Timestamps = new Timestamps()
    //            {
    //                Start = DateTime.UtcNow.AddSeconds(1)
    //            }
    //        });
    //    }
    //}
}
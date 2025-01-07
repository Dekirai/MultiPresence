using DiscordRPC;
using MultiPresence.Models.RE6;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE6
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212349543463518268");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("BH6")[0];
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
            Process[] game = Process.GetProcessesByName("BH6");
            if (game.Length > 0)
            {
                int stage_get = Hypervisor.Read<short>(Hypervisor.GetPointer32(0x1466884, [0xA422C]), true);
                int state_get = Hypervisor.Read<short>(Hypervisor.GetPointer32(0x1466884, [0xA4228]), true);
                var stagevalue = await Stages.GetStage(stage_get);

                string[] stage = stagevalue.Split(':');

                var placeholders = new Dictionary<string, object>
                    {
                        { "chapter", stage[0] },
                        { "room", stage[1] }
                    };

                if (state_get == 0)
                {
                    string details = updater.UpdateDetails("Resident Evil 6", placeholders, "Booting");
                    string state = updater.UpdateState("Resident Evil 6", placeholders, "Booting");
                    string largeasset = updater.UpdateLargeAsset("Resident Evil 6", placeholders, "Booting");
                    string largeassettext = updater.UpdateLargeAssetText("Resident Evil 6", placeholders, "Booting");
                    string smallasset = updater.UpdateSmallAsset("Resident Evil 6", placeholders, "Booting");
                    string smallassettext = updater.UpdateSmallAssetText("Resident Evil 6", placeholders, "Booting");
                    string button1text = updater.UpdateButton1Text("Resident Evil 6", placeholders, "Booting");
                    string button2text = updater.UpdateButton2Text("Resident Evil 6", placeholders, "Booting");
                    string button1url = updater.UpdateButton1URL("Resident Evil 6", placeholders, "Booting");
                    string button2url = updater.UpdateButton2URL("Resident Evil 6", placeholders, "Booting");
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
                else if (state_get > 1 && state_get < 9)
                {
                    string details = updater.UpdateDetails("Resident Evil 6", placeholders, "Ingame");
                    string state = updater.UpdateState("Resident Evil 6", placeholders, "Ingame");
                    string largeasset = updater.UpdateLargeAsset("Resident Evil 6", placeholders, "Ingame");
                    string largeassettext = updater.UpdateLargeAssetText("Resident Evil 6", placeholders, "Ingame");
                    string smallasset = updater.UpdateSmallAsset("Resident Evil 6", placeholders, "Ingame");
                    string smallassettext = updater.UpdateSmallAssetText("Resident Evil 6", placeholders, "Ingame");
                    string button1text = updater.UpdateButton1Text("Resident Evil 6", placeholders, "Ingame");
                    string button2text = updater.UpdateButton2Text("Resident Evil 6", placeholders, "Ingame");
                    string button1url = updater.UpdateButton1URL("Resident Evil 6", placeholders, "Ingame");
                    string button2url = updater.UpdateButton2URL("Resident Evil 6", placeholders, "Ingame");
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
                else if (state_get == 20)
                {
                    string details = updater.UpdateDetails("Resident Evil 6", placeholders, "Main_Menu");
                    string state = updater.UpdateState("Resident Evil 6", placeholders, "Main_Menu");
                    string largeasset = updater.UpdateLargeAsset("Resident Evil 6", placeholders, "Main_Menu");
                    string largeassettext = updater.UpdateLargeAssetText("Resident Evil 6", placeholders, "Main_Menu");
                    string smallasset = updater.UpdateSmallAsset("Resident Evil 6", placeholders, "Main_Menu");
                    string smallassettext = updater.UpdateSmallAssetText("Resident Evil 6", placeholders, "Main_Menu");
                    string button1text = updater.UpdateButton1Text("Resident Evil 6", placeholders, "Main_Menu");
                    string button2text = updater.UpdateButton2Text("Resident Evil 6", placeholders, "Main_Menu");
                    string button1url = updater.UpdateButton1URL("Resident Evil 6", placeholders, "Main_Menu");
                    string button2url = updater.UpdateButton2URL("Resident Evil 6", placeholders, "Main_Menu");
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
                else if (state_get == 10)
                {
                    string details = updater.UpdateDetails("Resident Evil 6", placeholders, "Saving");
                    string state = updater.UpdateState("Resident Evil 6", placeholders, "Saving");
                    string largeasset = updater.UpdateLargeAsset("Resident Evil 6", placeholders, "Saving");
                    string largeassettext = updater.UpdateLargeAssetText("Resident Evil 6", placeholders, "Saving");
                    string smallasset = updater.UpdateSmallAsset("Resident Evil 6", placeholders, "Saving");
                    string smallassettext = updater.UpdateSmallAssetText("Resident Evil 6", placeholders, "Saving");
                    string button1text = updater.UpdateButton1Text("Resident Evil 6", placeholders, "Saving");
                    string button2text = updater.UpdateButton2Text("Resident Evil 6", placeholders, "Saving");
                    string button1url = updater.UpdateButton1URL("Resident Evil 6", placeholders, "Saving");
                    string button2url = updater.UpdateButton2URL("Resident Evil 6", placeholders, "Saving");
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
                else if (state_get > 20)
                {
                    string details = updater.UpdateDetails("Resident Evil 6", placeholders, "Cutscene");
                    string state = updater.UpdateState("Resident Evil 6", placeholders, "Cutscene");
                    string largeasset = updater.UpdateLargeAsset("Resident Evil 6", placeholders, "Cutscene");
                    string largeassettext = updater.UpdateLargeAssetText("Resident Evil 6", placeholders, "Cutscene");
                    string smallasset = updater.UpdateSmallAsset("Resident Evil 6", placeholders, "Cutscene");
                    string smallassettext = updater.UpdateSmallAssetText("Resident Evil 6", placeholders, "Cutscene");
                    string button1text = updater.UpdateButton1Text("Resident Evil 6", placeholders, "Cutscene");
                    string button2text = updater.UpdateButton2Text("Resident Evil 6", placeholders, "Cutscene");
                    string button1url = updater.UpdateButton1URL("Resident Evil 6", placeholders, "Cutscene");
                    string button2url = updater.UpdateButton2URL("Resident Evil 6", placeholders, "Cutscene");
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

                discord.UpdateLargeAsset($"logo", $"Resident Evil 6");


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
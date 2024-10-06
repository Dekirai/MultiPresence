using DiscordRPC;
using Memory;
using MultiPresence.Models.VS;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class VS
    {
        static Mem mem = new Mem();
        static string process = "VampireSurvivors";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static string adventure = "";

        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1283783524423438409");
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
            List<string> modesList = new List<string>();
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                int characterid = mem.ReadInt("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x44");
                int characterid_adventure = mem.ReadInt("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x44");
                int stageid = mem.ReadInt("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x48");
                int stageid_adventure = mem.ReadInt("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x48");
                float coins = mem.ReadFloat("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x70");
                float coins_adventure = mem.ReadFloat("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x70");
                float coinsingame = mem.ReadFloat("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x74");
                float coinsingame_adventure = mem.ReadFloat("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x74");
                int kills = mem.ReadInt("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x78");
                int kills_adventure = mem.ReadInt("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x78");
                float health = mem.ReadFloat("GameAssembly.dll+049551E8,0xB8,0x0,0xF0,0x98,0x18,0x28,0x188");
                int level = mem.ReadInt("GameAssembly.dll+049551E8,0xB8,0x0,0xF0,0x98,0x18,0x28,0x18C");
                float time = mem.ReadFloat("GameAssembly.dll+049630C0,0x80,0x78,0x48,0x40,0xB8,0x0,0x360");
                int isIngame = mem.ReadByte("UnityPlayer.dll+1B32778");
                int isHyper = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x4C");
                int isHurry = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x4D");
                int hasArcanas = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x4E");
                int hasLimitbreak = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x4F");
                int isInverse = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x50");
                int isEndless = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x51");
                int hasRandomEvents = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x58");
                int hasRandomItems = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x50,0x59");
                int isHyper_adventure = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x4C");
                int isHurry_adventure = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x4D");
                int hasArcanas_adventure = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x4E");
                int hasLimitbreak_adventure = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x4F");
                int isInverse_adventure = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x50");
                int isEndless_adventure = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x51");
                int hasRandomEvents_adventure = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x58");
                int hasRandomItems_adventure = mem.ReadByte("GameAssembly.dll+048F8858,0x78,0xB8,0x0,0x40,0x10,0x60,0x59");

                TimeSpan timeSpan = TimeSpan.FromSeconds(time);
                string formattedTime = string.Format("{0:D2}:{1:D2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);

                var character = await Characters.GetCharacter(characterid);
                var character_adventure = await Characters.GetCharacter(characterid_adventure);
                var stage = await Stages.GetStages(stageid);
                var stage_adventure = await Stages.GetStages(stageid_adventure);

                if (isHyper == 1 || isHyper_adventure == 1)
                    modesList.Add("Hyper");
                if (isHurry == 1 || isHurry_adventure == 1)
                    modesList.Add("Hurry");
                if (hasArcanas == 1 || hasArcanas_adventure == 1)
                    modesList.Add("Arcanas");
                if (hasLimitbreak == 1 || hasLimitbreak_adventure == 1)
                    modesList.Add("Limit Break");
                if (isInverse == 1 || isInverse_adventure == 1)
                    modesList.Add("Inverse");
                if (isEndless == 1 || isEndless_adventure == 1)
                    modesList.Add("Endless");
                if (hasRandomEvents == 1 || hasRandomEvents_adventure == 1)
                    modesList.Add("Random Events");
                if (hasRandomItems == 1 || hasRandomItems_adventure == 1)
                    modesList.Add("Random LevelUp");

                string modes = string.Join(", ", modesList);
                if (modesList.Count == 0)
                    modes = "None";

                try
                {

                    if (stageid_adventure >= 1001 && stageid_adventure <= 1007)
                        adventure = "Legacy of the Moonspell";
                    else if (stageid_adventure >= 1011 && stageid_adventure <= 1016)
                        adventure = "A Garlic Paradise";
                    else if (stageid_adventure >= 1021 && stageid_adventure <= 1026)
                        adventure = "World of Light and Dark";
                    else if (stageid_adventure >= 1031 && stageid_adventure <= 1036)
                        adventure = "Emergency Meeting";
                    else if (stageid_adventure >= 1041 && stageid_adventure <= 1046)
                        adventure = "Operation Guns";
                    else
                        adventure = "Adventure";

                    if (isIngame == 0)
                    {
                        if (stageid_adventure > 0)
                        {
                            var placeholders = new Dictionary<string, object>
                            {
                                { "character", character_adventure },
                                { "stage", stage_adventure },
                                { "coins", (int)Math.Floor(coins_adventure) },
                                { "adventure", adventure },
                                { "modes", modes }
                            };
                            string details = updater.UpdateDetails("Vampire Survivors", placeholders, "Default_Adventure");
                            string state = updater.UpdateState("Vampire Survivors", placeholders, "Default_Adventure");
                            string largeasset = updater.UpdateLargeAsset("Vampire Survivors", placeholders, "Default_Adventure");
                            string largeassettext = updater.UpdateLargeAssetText("Vampire Survivors", placeholders, "Default_Adventure");
                            string smallasset = updater.UpdateSmallAsset("Vampire Survivors", placeholders, "Default_Adventure");
                            string smallassettext = updater.UpdateSmallAssetText("Vampire Survivors", placeholders, "Default_Adventure");
                            string button1text = updater.UpdateButton1Text("Vampire Survivors", placeholders, "Default_Adventure");
                            string button2text = updater.UpdateButton2Text("Vampire Survivors", placeholders, "Default_Adventure");
                            string button1url = updater.UpdateButton1URL("Vampire Survivors", placeholders, "Default_Adventure");
                            string button2url = updater.UpdateButton2URL("Vampire Survivors", placeholders, "Default_Adventure");
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
                            var placeholders = new Dictionary<string, object>
                            {
                                { "character", character },
                                { "stage", stage },
                                { "coins", (int)Math.Floor(coins) },
                                { "adventure", adventure },
                                { "modes", modes }
                            };
                            string details = updater.UpdateDetails("Vampire Survivors", placeholders);
                            string state = updater.UpdateState("Vampire Survivors", placeholders);
                            string largeasset = updater.UpdateLargeAsset("Vampire Survivors", placeholders);
                            string largeassettext = updater.UpdateLargeAssetText("Vampire Survivors", placeholders);
                            string smallasset = updater.UpdateSmallAsset("Vampire Survivors", placeholders);
                            string smallassettext = updater.UpdateSmallAssetText("Vampire Survivors", placeholders);
                            string button1text = updater.UpdateButton1Text("Vampire Survivors", placeholders);
                            string button2text = updater.UpdateButton2Text("Vampire Survivors", placeholders);
                            string button1url = updater.UpdateButton1URL("Vampire Survivors", placeholders);
                            string button2url = updater.UpdateButton2URL("Vampire Survivors", placeholders);
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
                        if (stageid_adventure > 0)
                        {
                            var placeholders = new Dictionary<string, object>
                            {
                                { "character", character_adventure },
                                { "stage", stage_adventure },
                                { "coins", (int)Math.Floor(coins_adventure) },
                                { "coinsingame", (int)Math.Floor(coinsingame_adventure) },
                                { "kills", kills_adventure },
                                { "health", (int)Math.Floor(health) },
                                { "level", level },
                                { "time", formattedTime },
                                { "adventure", adventure },
                                { "modes", modes }
                            };
                            string details = updater.UpdateDetails("Vampire Survivors", placeholders, "Ingame_Adventure");
                            string state = updater.UpdateState("Vampire Survivors", placeholders, "Ingame_Adventure");
                            string largeasset = updater.UpdateLargeAsset("Vampire Survivors", placeholders, "Ingame_Adventure");
                            string largeassettext = updater.UpdateLargeAssetText("Vampire Survivors", placeholders, "Ingame_Adventure");
                            string smallasset = updater.UpdateSmallAsset("Vampire Survivors", placeholders, "Ingame_Adventure");
                            string smallassettext = updater.UpdateSmallAssetText("Vampire Survivors", placeholders, "Ingame_Adventure");
                            string button1text = updater.UpdateButton1Text("Vampire Survivors", placeholders, "Ingame_Adventure");
                            string button2text = updater.UpdateButton2Text("Vampire Survivors", placeholders, "Ingame_Adventure");
                            string button1url = updater.UpdateButton1URL("Vampire Survivors", placeholders, "Ingame_Adventure");
                            string button2url = updater.UpdateButton2URL("Vampire Survivors", placeholders, "Ingame_Adventure");
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
                            var placeholders = new Dictionary<string, object>
                            {
                                { "character", character },
                                { "stage", stage },
                                { "coins", (int)Math.Floor(coins) },
                                { "coinsingame", (int)Math.Floor(coinsingame) },
                                { "kills", kills },
                                { "health", (int)Math.Floor(health) },
                                { "level", level },
                                { "time", formattedTime },
                                { "modes", modes }
                            };
                            string details = updater.UpdateDetails("Vampire Survivors", placeholders, "Ingame");
                            string state = updater.UpdateState("Vampire Survivors", placeholders, "Ingame");
                            string largeasset = updater.UpdateLargeAsset("Vampire Survivors", placeholders, "Ingame");
                            string largeassettext = updater.UpdateLargeAssetText("Vampire Survivors", placeholders, "Ingame");
                            string smallasset = updater.UpdateSmallAsset("Vampire Survivors", placeholders, "Ingame");
                            string smallassettext = updater.UpdateSmallAssetText("Vampire Survivors", placeholders, "Ingame");
                            string button1text = updater.UpdateButton1Text("Vampire Survivors", placeholders, "Ingame");
                            string button2text = updater.UpdateButton2Text("Vampire Survivors", placeholders, "Ingame");
                            string button1url = updater.UpdateButton1URL("Vampire Survivors", placeholders, "Ingame");
                            string button2url = updater.UpdateButton2URL("Vampire Survivors", placeholders, "Ingame");
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
                    discord.UpdateLargeAsset("logo", "Vampire Survivors");
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

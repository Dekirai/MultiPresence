using DiscordRPC;
using MultiPresence.Models.VS;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class VS
    {
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
            try
            {
                var _myProcess = Process.GetProcessesByName("VampireSurvivors")[0];
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
            List<string> modesList = new List<string>();
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                var characterid = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x44], false, "GameAssembly.dll"), true);
                var characterid_adventure = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x44], false, "GameAssembly.dll"), true);
                var stageid = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x48], false, "GameAssembly.dll"), true);
                var stageid_adventure = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x48], false, "GameAssembly.dll"), true);
                var coins = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x78], false, "GameAssembly.dll"), true);
                var coins_adventure = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x78], false, "GameAssembly.dll"), true);
                var coinsingame = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x7C], false, "GameAssembly.dll"), true);
                var coinsingame_adventure = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x7C], false, "GameAssembly.dll"), true);
                var kills = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x80], false, "GameAssembly.dll"), true);
                var kills_adventure = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x80], false, "GameAssembly.dll"), true);
                var health = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x04C559B0, [0xB8, 0x0, 0xF0, 0x18, 0x30, 0x10, 0x1C0], false, "GameAssembly.dll"), true);
                var level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x04C559B0, [0xB8, 0x0, 0xF0, 0x18, 0x30, 0x10, 0x1C4], false, "GameAssembly.dll"), true);
                var time = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x04C559B0, [0xB8, 0x0, 0x10, 0xA8, 0x28, 0x398], false, "GameAssembly.dll"), true);
                var isIngame = Hypervisor.Read<byte>(0x4F897EC, false, "GameAssemblyw.dll");
                var isHyper = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x4C], false, "GameAssembly.dll"), true);
                var isHurry = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x4D], false, "GameAssembly.dll"), true);
                var hasArcanas = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x4E], false, "GameAssembly.dll"), true);
                var hasLimitbreak = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x4F], false, "GameAssembly.dll"), true);
                var isInverse = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x50], false, "GameAssembly.dll"), true);
                var isEndless = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x51], false, "GameAssembly.dll"), true);
                var hasRandomEvents = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x58], false, "GameAssembly.dll"), true);
                var hasRandomItems = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x50, 0x59], false, "GameAssembly.dll"), true);
                var isHyper_adventure = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x4C], false, "GameAssembly.dll"), true);
                var isHurry_adventure = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x4D], false, "GameAssembly.dll"), true);
                var hasArcanas_adventure = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x4E], false, "GameAssembly.dll"), true);
                var hasLimitbreak_adventure = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x4F], false, "GameAssembly.dll"), true);
                var isInverse_adventure = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x50], false, "GameAssembly.dll"), true);
                var isEndless_adventure = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x51], false, "GameAssembly.dll"), true);
                var hasRandomEvents_adventure = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x58], false, "GameAssembly.dll"), true);
                var hasRandomItems_adventure = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x04C55460, [0x78, 0xB8, 0x0, 0x40, 0x10, 0x60, 0x59], false, "GameAssembly.dll"), true);


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

                    if (time == 0)
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

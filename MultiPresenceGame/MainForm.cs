using DiscordRPC;
using Steamworks;
using System;
using System.Diagnostics;
using System.Text;

namespace MultiPresenceGame
{
    public partial class MainForm : Form
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater? updater;
        public MainForm()
        {
            InitializeComponent();
            updater = new DiscordStatusUpdater("config.json");
#if DEBUG
            File.WriteAllText("steam_appid.txt", "1217060");
            if (!SteamAPI.Init())
            {
                //Do nothing
            }
            int keyCount = SteamFriends.GetFriendRichPresenceKeyCount(SteamUser.GetSteamID());

            if (keyCount == 0)
            {
                MessageBox.Show("No Rich Presence keys found.");
            }
            else
            {
                for (int i = 0; i < keyCount; i++)
                {
                    string key = SteamFriends.GetFriendRichPresenceKeyByIndex(SteamUser.GetSteamID(), i);
                    string value = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), key);

                    MessageBox.Show($"Key: {key}, Value: {value}");
                }
            }
            //DoAction();
#else
            DoAction();
#endif
        }

        public static async void DoAction()
        {
            Process[] ow = Process.GetProcessesByName("Overwatch");
            Process[] tts = Process.GetProcessesByName("TemtemSwarm");
            Process[] hl = Process.GetProcessesByName("HogwartsLegacy");
            Process[] cod = Process.GetProcessesByName("cod");
            Process[] hk = Process.GetProcessesByName("Hello Kitty");
            if (ow.Length > 0)
            {
                discord = new DiscordRpcClient("1270342180623487089");
                InitializeDiscord();
                // Initialize Steamworks
                File.WriteAllText("steam_appid.txt", "2357570");
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                Thread thread = new Thread(RPCOW);
                thread.Start();
            }
            else if (tts.Length > 0)
            {
                discord = new DiscordRpcClient("1307417236113195038");
                InitializeDiscord();
                File.WriteAllText("steam_appid.txt", "2510960");
                // Initialize Steamworks
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                Thread thread = new Thread(RPCTTS);
                thread.Start();
            }
            else if (hl.Length > 0)
            {
                discord = new DiscordRpcClient("1324797968682979481");
                InitializeDiscord();
                File.WriteAllText("steam_appid.txt", "990080");
                // Initialize Steamworks
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                Thread thread = new Thread(RPCHL);
                thread.Start();
            }
            else if (cod.Length > 0)
            {
                discord = new DiscordRpcClient("1326219194316099696");
                InitializeDiscord();
                File.WriteAllText("steam_appid.txt", "1938090");
                // Initialize Steamworks
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                Thread thread = new Thread(RPCCOD);
                thread.Start();
            }
            else if (hk.Length > 0)
            {
                discord = new DiscordRpcClient("1334552623730266224");
                InitializeDiscord();
                File.WriteAllText("steam_appid.txt", "2495100");
                // Initialize Steamworks
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                Thread thread = new Thread(RPCHK);
                thread.Start();
            }
        }

        private static async void RPCOW()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("Overwatch");
                if (game.Length > 0)
                {
                    string presence = GetSteamRichPresence();
                    var placeholders = new Dictionary<string, object>
                    {
                        { "steam_display", presence }
                    };
                    try
                    {
                        string partyid = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "steam_player_group");
                        int partysize = int.Parse(SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "steam_player_group_size"));

                        if (partysize > 1)
                        {
                            discord.UpdateParty(new Party
                            {
                                ID = partyid,
                                Size = partysize,
                                Max = 6,
                            });
                            string details = updater.UpdateDetails("Overwatch", placeholders);
                            string state = updater.UpdateState("Overwatch", placeholders);
                            string largeasset = updater.UpdateLargeAsset("Overwatch", placeholders);
                            string largeassettext = updater.UpdateLargeAssetText("Overwatch", placeholders);
                            string smallasset = updater.UpdateSmallAsset("Overwatch", placeholders);
                            string smallassettext = updater.UpdateSmallAssetText("Overwatch", placeholders);
                            string button1text = updater.UpdateButton1Text("Overwatch", placeholders);
                            string button2text = updater.UpdateButton2Text("Overwatch", placeholders);
                            string button1url = updater.UpdateButton1URL("Overwatch", placeholders);
                            string button2url = updater.UpdateButton2URL("Overwatch", placeholders);
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
                            discord.UpdateParty(null);
                            string details = updater.UpdateDetails("Overwatch", placeholders);
                            string state = updater.UpdateState("Overwatch", placeholders);
                            string largeasset = updater.UpdateLargeAsset("Overwatch", placeholders);
                            string largeassettext = updater.UpdateLargeAssetText("Overwatch", placeholders);
                            string smallasset = updater.UpdateSmallAsset("Overwatch", placeholders);
                            string smallassettext = updater.UpdateSmallAssetText("Overwatch", placeholders);
                            string button1text = updater.UpdateButton1Text("Overwatch", placeholders);
                            string button2text = updater.UpdateButton2Text("Overwatch", placeholders);
                            string button1url = updater.UpdateButton1URL("Overwatch", placeholders);
                            string button2url = updater.UpdateButton2URL("Overwatch", placeholders);
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
                    catch
                    {
                        discord.UpdateParty(null);
                        string details = updater.UpdateDetails("Overwatch", placeholders);
                        string state = updater.UpdateState("Overwatch", placeholders);
                        string largeasset = updater.UpdateLargeAsset("Overwatch", placeholders);
                        string largeassettext = updater.UpdateLargeAssetText("Overwatch", placeholders);
                        string smallasset = updater.UpdateSmallAsset("Overwatch", placeholders);
                        string smallassettext = updater.UpdateSmallAssetText("Overwatch", placeholders);
                        string button1text = updater.UpdateButton1Text("Overwatch", placeholders);
                        string button2text = updater.UpdateButton2Text("Overwatch", placeholders);
                        string button1url = updater.UpdateButton1URL("Overwatch", placeholders);
                        string button2url = updater.UpdateButton2URL("Overwatch", placeholders);
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

                    await Task.Delay(3000); // Wait before checking again
                }
                else
                {
                    SteamFriends.ClearRichPresence();
                    File.WriteAllText("steam_appid.txt", "");
                    SteamAPI.Shutdown();

                    discord.Deinitialize();
                    Environment.Exit(0);
                    break;
                }
            }
        }

        private static async void RPCTTS()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("TemtemSwarm");
                if (game.Length > 0)
                {
                    string presence = GetSteamRichPresence();
                    try
                    {
                        string temtem = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "temtem");
                        string stage = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "stage");
                        string round = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "round");

                        if (temtem.Length > 0)
                        {
                            var placeholders = new Dictionary<string, object>
                            {
                                { "steam_display", presence },
                                { "temtem", temtem },
                                { "stage", stage },
                                { "round", round },
                            };
                            string details = updater.UpdateDetails("Temtem Swarm", placeholders, "Ingame");
                            string state = updater.UpdateState("Temtem Swarm", placeholders, "Ingame");
                            string largeasset = updater.UpdateLargeAsset("Temtem Swarm", placeholders, "Ingame");
                            string largeassettext = updater.UpdateLargeAssetText("Temtem Swarm", placeholders, "Ingame");
                            string smallasset = updater.UpdateSmallAsset("Temtem Swarm", placeholders, "Ingame");
                            string smallassettext = updater.UpdateSmallAssetText("Temtem Swarm", placeholders, "Ingame");
                            string button1text = updater.UpdateButton1Text("Temtem Swarm", placeholders, "Ingame");
                            string button2text = updater.UpdateButton2Text("Temtem Swarm", placeholders, "Ingame");
                            string button1url = updater.UpdateButton1URL("Temtem Swarm", placeholders, "Ingame");
                            string button2url = updater.UpdateButton2URL("Temtem Swarm", placeholders, "Ingame");
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
                                { "steam_display", presence }
                            };
                            string details = updater.UpdateDetails("Temtem Swarm", placeholders);
                            string state = updater.UpdateState("Temtem Swarm", placeholders);
                            string largeasset = updater.UpdateLargeAsset("Temtem Swarm", placeholders);
                            string largeassettext = updater.UpdateLargeAssetText("Temtem Swarm", placeholders);
                            string smallasset = updater.UpdateSmallAsset("Temtem Swarm", placeholders);
                            string smallassettext = updater.UpdateSmallAssetText("Temtem Swarm", placeholders);
                            string button1text = updater.UpdateButton1Text("Temtem Swarm", placeholders);
                            string button2text = updater.UpdateButton2Text("Temtem Swarm", placeholders);
                            string button1url = updater.UpdateButton1URL("Temtem Swarm", placeholders);
                            string button2url = updater.UpdateButton2URL("Temtem Swarm", placeholders);
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
                    catch
                    {
                        var placeholders = new Dictionary<string, object>
                        {
                            { "steam_display", presence }
                        };
                        string details = updater.UpdateDetails("Temtem Swarm", placeholders);
                        string state = updater.UpdateState("Temtem Swarm", placeholders);
                        string largeasset = updater.UpdateLargeAsset("Temtem Swarm", placeholders);
                        string largeassettext = updater.UpdateLargeAssetText("Temtem Swarm", placeholders);
                        string smallasset = updater.UpdateSmallAsset("Temtem Swarm", placeholders);
                        string smallassettext = updater.UpdateSmallAssetText("Temtem Swarm", placeholders);
                        string button1text = updater.UpdateButton1Text("Temtem Swarm", placeholders);
                        string button2text = updater.UpdateButton2Text("Temtem Swarm", placeholders);
                        string button1url = updater.UpdateButton1URL("Temtem Swarm", placeholders);
                        string button2url = updater.UpdateButton2URL("Temtem Swarm", placeholders);
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

                    await Task.Delay(3000); // Wait before checking again
                }
                else
                {
                    SteamFriends.ClearRichPresence();
                    File.WriteAllText("steam_appid.txt", "");
                    SteamAPI.Shutdown();

                    discord.Deinitialize();
                    Environment.Exit(0);
                    break;
                }
            }
        }

        private static async void RPCHL()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("HogwartsLegacy");
                if (game.Length > 0)
                {
                    string presence = GetSteamRichPresence();
                    var placeholders = new Dictionary<string, object>
                        {
                            { "steam_display", presence }
                        };
                    string details = updater.UpdateDetails("Hogwarts Legacy", placeholders);
                    string state = updater.UpdateState("Hogwarts Legacy", placeholders);
                    string largeasset = updater.UpdateLargeAsset("Hogwarts Legacy", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("Hogwarts Legacy", placeholders);
                    string smallasset = updater.UpdateSmallAsset("Hogwarts Legacy", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("Hogwarts Legacy", placeholders);
                    string button1text = updater.UpdateButton1Text("Hogwarts Legacy", placeholders);
                    string button2text = updater.UpdateButton2Text("Hogwarts Legacy", placeholders);
                    string button1url = updater.UpdateButton1URL("Hogwarts Legacy", placeholders);
                    string button2url = updater.UpdateButton2URL("Hogwarts Legacy", placeholders);
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

                    await Task.Delay(3000); // Wait before checking again
                }
                else
                {
                    SteamFriends.ClearRichPresence();
                    File.WriteAllText("steam_appid.txt", "");
                    SteamAPI.Shutdown();

                    discord.Deinitialize();
                    Environment.Exit(0);
                    break;
                }
            }
        }

        private static async void RPCCOD()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("cod");
                if (game.Length > 0)
                {
                    string presence = GetSteamRichPresence();

                    if (presence.Length >= 3)
                    {
                        var placeholders = new Dictionary<string, object>
                        {
                            { "steam_display", presence }
                        };
                        string details = updater.UpdateDetails("Call of Duty", placeholders);
                        string state = updater.UpdateState("Call of Duty", placeholders);
                        string largeasset = updater.UpdateLargeAsset("Call of Duty", placeholders);
                        string largeassettext = updater.UpdateLargeAssetText("Call of Duty", placeholders);
                        string smallasset = updater.UpdateSmallAsset("Call of Duty", placeholders);
                        string smallassettext = updater.UpdateSmallAssetText("Call of Duty", placeholders);
                        string button1text = updater.UpdateButton1Text("Call of Duty", placeholders);
                        string button2text = updater.UpdateButton2Text("Call of Duty", placeholders);
                        string button1url = updater.UpdateButton1URL("Call of Duty", placeholders);
                        string button2url = updater.UpdateButton2URL("Call of Duty", placeholders);
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
                        try
                        {
                            int mapkey = int.Parse(SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "mapname"));
                            int modekey = int.Parse(SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "gamemode"));

                            string mode = "";
                            string map = "";

                            if (mapkey > 0)
                            {
                                if (modekey == 1371735337)
                                    mode = "Zombies (Directed)";
                                else if (modekey == 1803630921)
                                    mode = "Zombies (Standard)";
                                else if (modekey == 1751835769)
                                    mode = "Zombies (Training Course)";

                                if (mapkey == 1320634394)
                                    map = "Liberty Falls";
                                else if (mapkey == 211748868)
                                    map = "Terminus";
                                else if (mapkey == 1738814346)
                                    map = "Citadelle Des Morts";
                                else if (mapkey == 51790153)
                                    map = "The Tomb";

                                var placeholders = new Dictionary<string, object>
                                {
                                    { "steam_display", presence },
                                    { "mode", mode },
                                    { "map", map },
                                };
                                string details = updater.UpdateDetails("Call of Duty", placeholders, "Zombies");
                                string state = updater.UpdateState("Call of Duty", placeholders, "Zombies");
                                string largeasset = updater.UpdateLargeAsset("Call of Duty", placeholders, "Zombies");
                                string largeassettext = updater.UpdateLargeAssetText("Call of Duty", placeholders, "Zombies");
                                string smallasset = updater.UpdateSmallAsset("Call of Duty", placeholders, "Zombies");
                                string smallassettext = updater.UpdateSmallAssetText("Call of Duty", placeholders, "Zombies");
                                string button1text = updater.UpdateButton1Text("Call of Duty", placeholders, "Zombies");
                                string button2text = updater.UpdateButton2Text("Call of Duty", placeholders, "Zombies");
                                string button1url = updater.UpdateButton1URL("Call of Duty", placeholders, "Zombies");
                                string button2url = updater.UpdateButton2URL("Call of Duty", placeholders, "Zombies");
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
                                    { "steam_display", presence }
                                };
                                string details = updater.UpdateDetails("Call of Duty", placeholders);
                                string state = updater.UpdateState("Call of Duty", placeholders);
                                string largeasset = updater.UpdateLargeAsset("Call of Duty", placeholders);
                                string largeassettext = updater.UpdateLargeAssetText("Call of Duty", placeholders);
                                string smallasset = updater.UpdateSmallAsset("Call of Duty", placeholders);
                                string smallassettext = updater.UpdateSmallAssetText("Call of Duty", placeholders);
                                string button1text = updater.UpdateButton1Text("Call of Duty", placeholders);
                                string button2text = updater.UpdateButton2Text("Call of Duty", placeholders);
                                string button1url = updater.UpdateButton1URL("Call of Duty", placeholders);
                                string button2url = updater.UpdateButton2URL("Call of Duty", placeholders);
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
                        catch
                        {
                            var placeholders = new Dictionary<string, object>
                            {
                                { "steam_display", presence }
                            };
                            string details = updater.UpdateDetails("Call of Duty", placeholders);
                            string state = updater.UpdateState("Call of Duty", placeholders);
                            string largeasset = updater.UpdateLargeAsset("Call of Duty", placeholders);
                            string largeassettext = updater.UpdateLargeAssetText("Call of Duty", placeholders);
                            string smallasset = updater.UpdateSmallAsset("Call of Duty", placeholders);
                            string smallassettext = updater.UpdateSmallAssetText("Call of Duty", placeholders);
                            string button1text = updater.UpdateButton1Text("Call of Duty", placeholders);
                            string button2text = updater.UpdateButton2Text("Call of Duty", placeholders);
                            string button1url = updater.UpdateButton1URL("Call of Duty", placeholders);
                            string button2url = updater.UpdateButton2URL("Call of Duty", placeholders);
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

                    await Task.Delay(3000);
                }
                else
                {
                    SteamFriends.ClearRichPresence();
                    File.WriteAllText("steam_appid.txt", "");
                    SteamAPI.Shutdown();

                    discord.Deinitialize();
                    Environment.Exit(0);
                    break;
                }
            }
        }

        private static async void RPCHK()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("Hello Kitty");
                if (game.Length > 0)
                {
                    string presence = GetSteamRichPresence();
                    var placeholders = new Dictionary<string, object>
                    {
                        { "steam_display", presence }
                    };
                    try
                    {
                        string partyid = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "steam_player_group");
                        int partysize = int.Parse(SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "steam_player_group_size"));

                        if (partysize > 1)
                        {
                            discord.UpdateParty(new Party
                            {
                                ID = partyid,
                                Size = partysize,
                                Max = 2,
                            });
                            string details = updater.UpdateDetails("Hello Kitty", placeholders);
                            string state = updater.UpdateState("Hello Kitty", placeholders);
                            string largeasset = updater.UpdateLargeAsset("Hello Kitty", placeholders);
                            string largeassettext = updater.UpdateLargeAssetText("Hello Kitty", placeholders);
                            string smallasset = updater.UpdateSmallAsset("Hello Kitty", placeholders);
                            string smallassettext = updater.UpdateSmallAssetText("Hello Kitty", placeholders);
                            string button1text = updater.UpdateButton1Text("Hello Kitty", placeholders);
                            string button2text = updater.UpdateButton2Text("Hello Kitty", placeholders);
                            string button1url = updater.UpdateButton1URL("Hello Kitty", placeholders);
                            string button2url = updater.UpdateButton2URL("Hello Kitty", placeholders);
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
                            discord.UpdateParty(null);
                            string details = updater.UpdateDetails("OverwHello Kittyatch", placeholders);
                            string state = updater.UpdateState("Hello Kitty", placeholders);
                            string largeasset = updater.UpdateLargeAsset("Hello Kitty", placeholders);
                            string largeassettext = updater.UpdateLargeAssetText("Hello Kitty", placeholders);
                            string smallasset = updater.UpdateSmallAsset("Hello Kitty", placeholders);
                            string smallassettext = updater.UpdateSmallAssetText("Hello Kitty", placeholders);
                            string button1text = updater.UpdateButton1Text("Hello Kitty", placeholders);
                            string button2text = updater.UpdateButton2Text("Hello Kitty", placeholders);
                            string button1url = updater.UpdateButton1URL("Hello Kitty", placeholders);
                            string button2url = updater.UpdateButton2URL("Hello Kitty", placeholders);
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
                    catch
                    {
                        discord.UpdateParty(null);
                        string details = updater.UpdateDetails("Hello Kitty", placeholders);
                        string state = updater.UpdateState("Hello Kitty", placeholders);
                        string largeasset = updater.UpdateLargeAsset("Hello Kitty", placeholders);
                        string largeassettext = updater.UpdateLargeAssetText("Hello Kitty", placeholders);
                        string smallasset = updater.UpdateSmallAsset("Hello Kitty", placeholders);
                        string smallassettext = updater.UpdateSmallAssetText("Hello Kitty", placeholders);
                        string button1text = updater.UpdateButton1Text("Hello Kitty", placeholders);
                        string button2text = updater.UpdateButton2Text("Hello Kitty", placeholders);
                        string button1url = updater.UpdateButton1URL("Hello Kitty", placeholders);
                        string button2url = updater.UpdateButton2URL("Hello Kitty", placeholders);
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

                    await Task.Delay(3000); // Wait before checking again
                }
                else
                {
                    SteamFriends.ClearRichPresence();
                    File.WriteAllText("steam_appid.txt", "");
                    SteamAPI.Shutdown();

                    discord.Deinitialize();
                    Environment.Exit(0);
                    break;
                }
            }
        }

        private static string GetSteamRichPresence()
        {
            string key = "steam_display"; // Key varies depending on the game
            string richPresence = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), key);

            return string.IsNullOrEmpty(richPresence) ? "" : richPresence;
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

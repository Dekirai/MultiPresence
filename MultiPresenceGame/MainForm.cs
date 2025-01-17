using DiscordRPC;
using Steamworks;
using System;
using System.Diagnostics;

namespace MultiPresenceGame
{
    public partial class MainForm : Form
    {
        private static DiscordRpcClient discord;
        public MainForm()
        {
            InitializeComponent();
            DoAction();
        }

        public static async void DoAction()
        {
            Process[] ow = Process.GetProcessesByName("Overwatch");
            Process[] tts = Process.GetProcessesByName("TemtemSwarm");
            Process[] hl = Process.GetProcessesByName("HogwartsLegacy");
            Process[] cod = Process.GetProcessesByName("cod");
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
        }

        private static async void RPCOW()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("Overwatch");
                if (game.Length > 0)
                {
                    string presence = GetSteamRichPresence();
                    discord.UpdateLargeAsset("logo", "Overwatch 2");
                    discord.UpdateDetails(presence);

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
                    discord.UpdateLargeAsset("logo", "Temtem: Swarm");
                    discord.UpdateDetails(presence);

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
                    discord.UpdateLargeAsset("logo", "Hogwarts Legacy");
                    discord.UpdateDetails(presence);

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

                    if (presence.Length >= 5)
                    {
                        discord.UpdateDetails(presence);
                        discord.UpdateState("");
                    }
                    else
                    {
                        try
                        {
                            int mapkey = int.Parse(SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "mapname"));
                            int modekey = int.Parse(SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "gamemode"));

                            if (mapkey > 0)
                            {
                                if (modekey == 1371735337)
                                    discord.UpdateDetails("Black Ops 6 - Zombies (Directed)"); //Geführt
                                else if (modekey == 1803630921)
                                    discord.UpdateDetails("Black Ops 6 - Zombies (Standard)");
                                else if (modekey == 2103910687)
                                    discord.UpdateDetails("Black Ops 6 - Zombies (Dead Light, Green Light)"); //Totes Licht, Grünes Licht
                                else if (modekey == 1803630921)
                                    discord.UpdateDetails("Black Ops 6 - Zombies (Training Course)"); //Trainingskurs

                                if (mapkey == 1320634394)
                                    discord.UpdateState("Playing on Liberty Falls");
                                else if (mapkey == 211748868)
                                    discord.UpdateState("Playing on Terminus");
                                else if (mapkey == 1738814346)
                                    discord.UpdateState("Playing on Citalle Des Morts");
                            }
                            else
                            {
                                discord.UpdateDetails(presence);
                                discord.UpdateState("");
                            }
                        }
                        catch
                        {
                            discord.UpdateDetails(presence);
                            discord.UpdateState("");
                        }
                    }

                    discord.UpdateLargeAsset("logo", "Call of Duty®");

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

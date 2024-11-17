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

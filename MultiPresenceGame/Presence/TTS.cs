using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresenceGame.Presence
{
    public class TTS
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;

        public static void DoAction()
        {
            Process[] cod = Process.GetProcessesByName("TemtemSwarm");
            if (cod.Length > 0)
            {
                discord = new DiscordRpcClient("1307417236113195038");
                InitializeDiscord();
                File.WriteAllText("Assets/steam_appid.txt", "2510960");
                // Initialize Steamworks
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                updater = new DiscordStatusUpdater("Assets/config/Temtem Swarm.json");
                Thread thread = new Thread(RPCTTS);
                thread.Start();
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

                        if (temtem.Length > 0)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Temtem Swarm", placeholders, "Ingame");
                        }
                        else
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Temtem Swarm", placeholders);
                        }
                    }
                    catch
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Temtem Swarm", placeholders);
                    }

                    await Task.Delay(3000); // Wait before checking again
                }
                else
                {
                    SteamFriends.ClearRichPresence();
                    File.WriteAllText("Assets/steam_appid.txt", "");
                    SteamAPI.Shutdown();

                    discord.Deinitialize();
                    Environment.Exit(0);
                    break;
                }
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            string presence = GetSteamRichPresence();
            string temtem = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "temtem");
            string stage = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "stage");
            string round = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "round");
            if (temtem.Length > 0)
            {
                return new Dictionary<string, object>
                {
                    { "steam_display", presence },
                    { "temtem", temtem },
                    { "stage", stage },
                    { "round", round },
                };
            }

            return new Dictionary<string, object>
            {
                { "steam_display", presence }
            };
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
        }
    }
}
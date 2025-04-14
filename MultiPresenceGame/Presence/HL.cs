using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresenceGame.Presence
{
    public class HL
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;

        public static void DoAction()
        {
            Process[] cod = Process.GetProcessesByName("HogwartsLegacy");
            if (cod.Length > 0)
            {
                discord = new DiscordRpcClient("1324797968682979481");
                InitializeDiscord();
                File.WriteAllText("Assets/steam_appid.txt", "990080");
                // Initialize Steamworks
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                updater = new DiscordStatusUpdater("Assets/config/Hogwarts Legacy.json");
                Thread thread = new Thread(RPCTTS);
                thread.Start();
            }
        }

        private static async void RPCTTS()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("HogwartsLegacy");
                if (game.Length > 0)
                {
                    string presence = GetSteamRichPresence();
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Hogwarts Legacy", placeholders);

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
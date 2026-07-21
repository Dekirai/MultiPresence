#nullable disable
using MultiPresenceGame.Runtime;
using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresenceGame.Presence
{
    public class HL
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;

        public static void DoAction()
        {
            if (SteamHostRuntime.IsProcessRunning("HogwartsLegacy"))
            {
                discord = new DiscordRpcClient("1324797968682979481");
                InitializeDiscord();
                SteamHostRuntime.WriteSteamAppId("990080");
                // Initialize Steamworks
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                updater = new DiscordStatusUpdater("Config/Hogwarts Legacy.json");
                SteamHostRuntime.Start(RPCTTS);
            }
        }

        private static async Task RPCTTS()
        {
            while (true)
            {
                if (SteamHostRuntime.IsProcessRunning("HogwartsLegacy"))
                {
                    string presence = GetSteamRichPresence();
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Hogwarts Legacy", placeholders);

                    await Task.Delay(3000); // Wait before checking again
                }
                else
                {
                    SteamFriends.ClearRichPresence();
                    SteamHostRuntime.ClearSteamAppId();
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

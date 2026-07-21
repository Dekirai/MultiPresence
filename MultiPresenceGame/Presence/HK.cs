#nullable disable
using MultiPresenceGame.Runtime;
using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresenceGame.Presence
{
    public class HK
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;

        public static void DoAction()
        {
            if (SteamHostRuntime.IsProcessRunning("Hello Kitty"))
            {
                discord = new DiscordRpcClient("1334552623730266224");
                InitializeDiscord();
                SteamHostRuntime.WriteSteamAppId("2495100");
                // Initialize Steamworks
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                updater = new DiscordStatusUpdater("Config/Hello Kitty Island Adventure.json");
                SteamHostRuntime.Start(RPC);
            }
        }

        private static async Task RPC()
        {
            while (true)
            {
                if (SteamHostRuntime.IsProcessRunning("Hello Kitty"))
                {
                    string presence = GetSteamRichPresence();

                    try
                    {
                        string partyid = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "steam_player_group");
                        int partysize = int.Parse(SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "steam_player_group_size"));

                        Party party = null;

                        if (partysize > 1)
                        {
                            party = new Party
                            {
                                ID = partyid,
                                Size = partysize,
                                Max = 2,
                            };
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Hello Kitty", placeholders, "Default", party);
                        }
                        else
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Hello Kitty", placeholders);
                        }
                    }
                    catch
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Hello Kitty", placeholders);
                    }

                    await Task.Delay(3000);
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

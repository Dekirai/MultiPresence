#nullable disable
using MultiPresenceGame.Runtime;
using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresenceGame.Presence
{
    public class TF2
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;

        public static void DoAction()
        {
            if (SteamHostRuntime.IsProcessRunning("tf_win64"))
            {
                discord = new DiscordRpcClient("1401813854002085921");
                InitializeDiscord();
                SteamHostRuntime.WriteSteamAppId("440");
                // Initialize Steamworks
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                updater = new DiscordStatusUpdater("Config/Team Fortress 2.json");
                SteamHostRuntime.Start(RPC);
            }
        }

        private static async Task RPC()
        {
            while (true)
            {
                if (SteamHostRuntime.IsProcessRunning("tf_win64"))
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
                                Max = 6,
                            };
                            if (presence.Contains("%currentmap%"))
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Team Fortress 2", placeholders, "Ingame", party);
                            }
                            else
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Team Fortress 2", placeholders, "Default", party);
                            }
                        }
                        else
                        {
                            if (presence.Contains("%currentmap%"))
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Team Fortress 2", placeholders, "Ingame");
                            }
                            else
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Team Fortress 2", placeholders);
                            }
                        }
                    }
                    catch
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Team Fortress 2", placeholders);
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
            string currentmap = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "currentmap");

            if (presence.Contains("%currentmap%"))
            {
                presence = presence.Replace(" – %currentmap%", "");
                return new Dictionary<string, object>
                {
                    { "steam_display", presence },
                    { "currentmap", currentmap }
                };
            }
            else
            {
                return new Dictionary<string, object>
                {
                    { "steam_display", presence }
                };
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
        }
    }
}

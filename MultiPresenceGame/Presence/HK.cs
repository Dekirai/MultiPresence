using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresenceGame.Presence
{
    public class HK
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;

        public static void DoAction()
        {
            Process[] cod = Process.GetProcessesByName("Hello Kitty");
            if (cod.Length > 0)
            {
                discord = new DiscordRpcClient("1334552623730266224");
                InitializeDiscord();
                File.WriteAllText("steam_appid.txt", "2495100");
                // Initialize Steamworks
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                updater = new DiscordStatusUpdater("config.json");
                Thread thread = new Thread(RPC);
                thread.Start();
            }
        }

        private static async void RPC()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("Hello Kitty");
                if (game.Length > 0)
                {
                    string presence = GetSteamRichPresence();

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
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Hello Kitty", placeholders);
                        }
                        else
                        {
                            discord.UpdateParty(null);
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Hello Kitty", placeholders);
                        }
                    }
                    catch
                    {
                        discord.UpdateParty(null);
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Hello Kitty", placeholders);
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
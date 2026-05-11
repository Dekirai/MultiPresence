using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresenceGame.Presence
{
    public class D4
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;

        public static void DoAction()
        {
            Process[] ow = Process.GetProcessesByName("Diablo IV");
            if (ow.Length > 0)
            {
                discord = new DiscordRpcClient("1502361064673906871");
                InitializeDiscord();
                File.WriteAllText("Assets/steam_appid.txt", "2344520");
                // Initialize Steamworks
                if (!SteamAPI.Init())
                {
                    //Do nothing
                }
                updater = new DiscordStatusUpdater("Assets/Config/Diablo IV.json");
                Thread thread = new Thread(RPC);
                thread.Start();
            }
        }

        private static async void RPC()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("Diablo IV");
                if (game.Length > 0)
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
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Diablo IV", placeholders, "Default", party);
                        }
                        else
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Diablo IV", placeholders);
                        }
                    }
                    catch
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Diablo IV", placeholders);
                    }

                    await Task.Delay(3000);
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
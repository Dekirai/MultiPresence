using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresenceGame.Presence
{
    public class COD
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;

        public static void DoAction()
        {
            Process[] cod = Process.GetProcessesByName("cod");
            if (cod.Length > 0)
            {
                discord = new DiscordRpcClient("1326219194316099696");
                InitializeDiscord();
                File.WriteAllText("steam_appid.txt", "1938090");
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
                Process[] game = Process.GetProcessesByName("cod");
                if (game.Length > 0)
                {
                    string presence = GetSteamRichPresence();

                    if (presence.Length >= 3)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Call of Duty", placeholders);
                    }
                    else
                    {
                        try
                        {
                            int mapkey = int.Parse(SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "mapname"));
                            int modekey = int.Parse(SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "gamemode"));

                            string mode = "";
                            string map = "";

                            if (modekey == 1371735337 || modekey == 1803630921 || modekey == 1751835769)
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Call of Duty", placeholders, "Zombies");
                            }
                            else
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Call of Duty", placeholders);
                            }
                        }
                        catch
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Call of Duty", placeholders);
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

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            string presence = GetSteamRichPresence();
            if (presence.Length >= 3)
            {
                return new Dictionary<string, object>
                {
                    { "steam_display", presence }
                };
            }
            else
            {
                int mapkey = int.Parse(SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "mapname"));
                int modekey = int.Parse(SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "gamemode"));

                string mode = "";
                string map = "";

                if (modekey == 1371735337 || modekey == 1803630921 || modekey == 1751835769)
                {
                    switch (modekey)
                    {
                        case 1371735337:
                            mode = "Zombies (Directed)";
                            break;
                        case 1803630921:
                            mode = "Zombies (Standard)";
                            break;
                        case 1751835769:
                            mode = "Zombies (Training Course)";
                            break;
                    }

                    switch (mapkey)
                    {
                        case 1320634394:
                            map = "Liberty Falls";
                            break;
                        case 211748868:
                            map = "Terminus";
                            break;
                        case 1738814346:
                            map = "Citadelle Des Morts";
                            break;
                        case 51790153:
                            map = "The Tomb";
                            break;
                        default:
                            map = "Unknown Map";
                            break;
                    }

                    return new Dictionary<string, object>
                    {
                        { "steam_display", presence },
                        { "mode", mode },
                        { "map", map }
                    };
                }
            }

            return new Dictionary<string, object>
            {
                { "steam_display", presence },
                { "mode", "Unknown Mode" },
                { "map", "Unknown Map" }
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
using DiscordRPC;
using Steamworks;

namespace MultiPresence.Steam.Presence
{
    public class TTS
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;

        public static void DoAction()
        {
            SteamHostRuntime.Start(RPC, Stop);
            discord = new DiscordRpcClient("1307417236113195038");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Temtem Swarm.json");
        }

        private static async Task RPC()
        {
            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
            string state = placeholders.ContainsKey("temtem") ? "Ingame" : "Default";
            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Temtem Swarm", placeholders, state);
        }

        private static Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            string presence = GetSteamRichPresence();
            var player = SteamUser.GetSteamID();
            string temtem = SteamFriends.GetFriendRichPresence(player, "temtem");
            if (temtem.Length > 0)
            {
                return Task.FromResult(new Dictionary<string, object>
                {
                    { "steam_display", presence },
                    { "temtem", temtem },
                    { "stage", SteamFriends.GetFriendRichPresence(player, "stage") },
                    { "round", SteamFriends.GetFriendRichPresence(player, "round") }
                });
            }

            return Task.FromResult(new Dictionary<string, object>
            {
                { "steam_display", presence }
            });
        }

        private static string GetSteamRichPresence()
        {
            string richPresence = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "steam_display");
            return string.IsNullOrEmpty(richPresence) ? "" : richPresence;
        }

        private static void Stop()
        {
            try
            {
                discord?.Dispose();
            }
            finally
            {
                updater?.Dispose();
                discord = null;
                updater = null;
            }
        }

        private static void InitializeDiscord()
        {
            discord?.Initialize();
        }
    }
}

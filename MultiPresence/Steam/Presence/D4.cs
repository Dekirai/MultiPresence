using DiscordRPC;
using Steamworks;
using System.Globalization;

namespace MultiPresence.Steam.Presence
{
    public class D4
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;

        public static void DoAction()
        {
            SteamHostRuntime.Start(RPC, Stop);
            discord = new DiscordRpcClient("1502361064673906871");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Diablo IV.json");
        }

        private static async Task RPC()
        {
            var player = SteamUser.GetSteamID();
            string partyid = SteamFriends.GetFriendRichPresence(player, "steam_player_group");
            string size = SteamFriends.GetFriendRichPresence(player, "steam_player_group_size");
            Party? party = null;
            if (int.TryParse(size, NumberStyles.None, CultureInfo.InvariantCulture, out var partysize) &&
                partysize is > 1 and <= 6 && !string.IsNullOrEmpty(partyid))
            {
                party = new Party()
                {
                    ID = partyid,
                    Size = partysize,
                    Max = 6
                };
            }

            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Diablo IV", placeholders, "Default", party);
        }

        private static Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            return Task.FromResult(new Dictionary<string, object>
            {
                { "steam_display", GetSteamRichPresence() }
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

using DiscordRPC;

namespace MultiPresenceGame
{
    public static class PlaceholderHelper
    {
        public static Timestamps _startTimestamp = Timestamps.Now; // Stores initial timestamp

        public static async Task<Dictionary<string, object>> GetPlaceholders(Func<Task<Dictionary<string, object>>> generatePlaceholders)
        {
            return await generatePlaceholders();
        }

        public static void UpdateDiscordStatus(DiscordRpcClient discord, DiscordStatusUpdater updater, string gameName, Dictionary<string, object> placeholders, string state = "Default")
        {
            string button1text = updater.UpdateButton1Text(gameName, placeholders, state);
            string button1url = updater.UpdateButton1URL(gameName, placeholders, state);
            string button2text = updater.UpdateButton2Text(gameName, placeholders, state);
            string button2url = updater.UpdateButton2URL(gameName, placeholders, state);

            var buttons = new List<DiscordRPC.Button>();

            if (!string.IsNullOrEmpty(button1url))
            {
                buttons.Add(new DiscordRPC.Button { Label = button1text, Url = button1url });
            }
            if (!string.IsNullOrEmpty(button2url))
            {
                buttons.Add(new DiscordRPC.Button { Label = button2text, Url = button2url });
            }

            // Set presence with buttons included

            discord.SetPresence(new RichPresence()
            {
                Details = updater.UpdateDetails(gameName, placeholders, state),
                State = updater.UpdateState(gameName, placeholders, state),
                Assets = new Assets()
                {
                    LargeImageKey = updater.UpdateLargeAsset(gameName, placeholders, state),
                    LargeImageText = updater.UpdateLargeAssetText(gameName, placeholders, state)
                },
                Timestamps = _startTimestamp,
                Buttons = buttons.Count > 0 ? buttons.ToArray() : null
            });
        }
    }
}

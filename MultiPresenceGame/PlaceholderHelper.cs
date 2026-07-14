using DiscordRPC;

namespace MultiPresenceGame
{
    public static class PlaceholderHelper
    {
        public static Timestamps _startTimestamp = Timestamps.Now;

        public static async Task<Dictionary<string, object>> GetPlaceholders(Func<Task<Dictionary<string, object>>> generatePlaceholders)
        {
            return await generatePlaceholders();
        }

        public static void UpdateDiscordStatus(DiscordRpcClient discord, DiscordStatusUpdater updater, string gameName, Dictionary<string, object> placeholders, string state = "Default", Party party = null)
        {
            var resolved = PresenceTemplateResolver.Resolve(updater, gameName, placeholders, state);
            var buttons = new List<DiscordRPC.Button>(2);

            if (!string.IsNullOrWhiteSpace(resolved.Button1Url))
                buttons.Add(new DiscordRPC.Button { Label = resolved.Button1Text, Url = resolved.Button1Url });

            if (!string.IsNullOrWhiteSpace(resolved.Button2Url))
                buttons.Add(new DiscordRPC.Button { Label = resolved.Button2Text, Url = resolved.Button2Url });

            discord.SetPresence(new RichPresence
            {
                Details = resolved.Details,
                State = resolved.State,
                Assets = new Assets
                {
                    LargeImageKey = resolved.LargeAsset,
                    LargeImageText = resolved.LargeAssetText,
                    SmallImageKey = resolved.SmallAsset,
                    SmallImageText = resolved.SmallAssetText
                },
                Timestamps = _startTimestamp,
                Buttons = buttons.Count > 0 ? buttons.ToArray() : null,
                Party = party
            });
        }
    }
}

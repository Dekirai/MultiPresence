using DiscordRPC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiPresence
{
    public static class PlaceholderHelper
    {
        public static async Task<Dictionary<string, object>> GetPlaceholders(Func<Task<Dictionary<string, object>>> generatePlaceholders)
        {
            return await generatePlaceholders();
        }

        public static void UpdateDiscordStatus(DiscordRpcClient discord, DiscordStatusUpdater updater, string gameName, Dictionary<string, object> placeholders, string state = "Default")
        {
            discord.UpdateLargeAsset(updater.UpdateLargeAsset(gameName, placeholders, state), updater.UpdateLargeAssetText(gameName, placeholders, state));
            discord.UpdateSmallAsset(updater.UpdateSmallAsset(gameName, placeholders, state), updater.UpdateSmallAssetText(gameName, placeholders, state));
            discord.UpdateDetails(updater.UpdateDetails(gameName, placeholders, state));
            discord.UpdateState(updater.UpdateState(gameName, placeholders, state));

            string button1text = updater.UpdateButton1Text(gameName, placeholders, state);
            string button1url = updater.UpdateButton1URL(gameName, placeholders, state);
            string button2text = updater.UpdateButton2Text(gameName, placeholders, state);
            string button2url = updater.UpdateButton2URL(gameName, placeholders, state);

            if (!string.IsNullOrEmpty(button1url))
            {
                var buttons = new List<DiscordRPC.Button> { new DiscordRPC.Button { Label = button1text, Url = button1url } };
                if (!string.IsNullOrEmpty(button2url))
                {
                    buttons.Add(new DiscordRPC.Button { Label = button2text, Url = button2url });
                }
                discord.UpdateButtons(buttons.ToArray());
            }
            else
            {
                discord.UpdateButtons(null);
            }
        }
    }
}

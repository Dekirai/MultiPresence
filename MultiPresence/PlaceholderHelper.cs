using DiscordRPC;
using MultiPresence.Infrastructure;

namespace MultiPresence;

public static class PlaceholderHelper
{
    public static Timestamps _startTimestamp = Timestamps.Now;

    public static async Task<Dictionary<string, object>> GetPlaceholders(
        Func<Task<Dictionary<string, object>>> generatePlaceholders)
    {
        ArgumentNullException.ThrowIfNull(generatePlaceholders);
        return await generatePlaceholders().ConfigureAwait(false);
    }

    public static void UpdateDiscordStatus(
        DiscordRpcClient? discord,
        DiscordStatusUpdater? updater,
        string gameName,
        Dictionary<string, object> placeholders,
        string state = "Default")
    {
        if (discord is null || updater is null)
        {
            AppLog.Warning($"Discord presence for '{gameName}' was skipped because it is not initialized.");
            return;
        }

        try
        {
            var buttons = CreateButtons(updater, gameName, placeholders, state);
            discord.SetPresence(new RichPresence
            {
                Details = Limit(updater.UpdateDetails(gameName, placeholders, state), 128),
                State = Limit(updater.UpdateState(gameName, placeholders, state), 128),
                Assets = new Assets
                {
                    LargeImageKey = EmptyToNull(Limit(
                        updater.UpdateLargeAsset(gameName, placeholders, state),
                        256)),
                    LargeImageText = EmptyToNull(Limit(
                        updater.UpdateLargeAssetText(gameName, placeholders, state),
                        128)),
                    SmallImageKey = EmptyToNull(Limit(
                        updater.UpdateSmallAsset(gameName, placeholders, state),
                        256)),
                    SmallImageText = EmptyToNull(Limit(
                        updater.UpdateSmallAssetText(gameName, placeholders, state),
                        128))
                },
                Timestamps = _startTimestamp,
                Buttons = buttons.Length == 0 ? null : buttons
            });
        }
        catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
        {
            AppLog.Warning($"Could not publish Discord presence for '{gameName}'.", exception);
        }
    }

    private static DiscordRPC.Button[] CreateButtons(
        DiscordStatusUpdater updater,
        string gameName,
        Dictionary<string, object> placeholders,
        string state)
    {
        var candidates = new[]
        {
            (
                updater.UpdateButton1Text(gameName, placeholders, state),
                updater.UpdateButton1URL(gameName, placeholders, state)),
            (
                updater.UpdateButton2Text(gameName, placeholders, state),
                updater.UpdateButton2URL(gameName, placeholders, state))
        };

        return candidates
            .Where(candidate =>
                !string.IsNullOrWhiteSpace(candidate.Item1) && IsValidWebUrl(candidate.Item2))
            .Select(candidate => new DiscordRPC.Button
            {
                Label = Limit(candidate.Item1, 32),
                Url = candidate.Item2
            })
            .ToArray();
    }

    private static bool IsValidWebUrl(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
        (uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
         uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase));

    private static string Limit(string? value, int maximumLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maximumLength)
        {
            return value ?? string.Empty;
        }

        return string.Concat(value.AsSpan(0, maximumLength - 1), "…");
    }

    private static string? EmptyToNull(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}

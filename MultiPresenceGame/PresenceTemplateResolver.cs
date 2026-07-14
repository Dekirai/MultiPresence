using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MultiPresenceGame;

public sealed record ResolvedPresence(
    string Details,
    string State,
    string LargeAsset,
    string LargeAssetText,
    string SmallAsset,
    string SmallAssetText,
    string Button1Text,
    string Button1Url,
    string Button2Text,
    string Button2Url);

public static class PresenceTemplateResolver
{
    public static ResolvedPresence Resolve(
        DiscordStatusUpdater legacyUpdater,
        string gameName,
        IReadOnlyDictionary<string, object> placeholders,
        string state)
    {
        var configPath = Path.Combine("Assets", "config", $"{gameName}.json");
        if (!File.Exists(configPath))
            return ResolveLegacy(legacyUpdater, gameName, placeholders, state);

        try
        {
            var config = new ConfigLoader(configPath).GetConfig();
            var gameConfig = config["Games"]?[gameName] as JObject;
            if (gameConfig is null)
                return ResolveLegacy(legacyUpdater, gameName, placeholders, state);

            var source = gameConfig[state] as JObject ?? gameConfig;
            return new ResolvedPresence(
                Apply(ReadTemplate(source, gameConfig, "Details"), placeholders),
                Apply(ReadTemplate(source, gameConfig, "State"), placeholders),
                Apply(ReadTemplate(source, gameConfig, "LargeAsset"), placeholders),
                Apply(ReadTemplate(source, gameConfig, "LargeAssetText"), placeholders),
                Apply(ReadTemplate(source, gameConfig, "SmallAsset"), placeholders),
                Apply(ReadTemplate(source, gameConfig, "SmallAssetText"), placeholders),
                Apply(ReadTemplate(source, gameConfig, "Button1Text"), placeholders),
                Apply(ReadTemplate(source, gameConfig, "Button1URL"), placeholders),
                Apply(ReadTemplate(source, gameConfig, "Button2Text"), placeholders),
                Apply(ReadTemplate(source, gameConfig, "Button2URL"), placeholders));
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Presence config resolution failed for {gameName}: {ex}");
            return ResolveLegacy(legacyUpdater, gameName, placeholders, state);
        }
    }

    private static string ReadTemplate(JObject source, JObject gameConfig, string propertyName)
        => source[propertyName]?.Value<string>()
            ?? gameConfig[propertyName]?.Value<string>()
            ?? string.Empty;

    private static string Apply(string template, IReadOnlyDictionary<string, object> placeholders)
    {
        foreach (var placeholder in placeholders)
        {
            template = Regex.Replace(
                template,
                $@"\{{\s*{Regex.Escape(placeholder.Key)}\s*\}}",
                _ => placeholder.Value?.ToString() ?? string.Empty,
                RegexOptions.CultureInvariant);
        }

        return template;
    }

    private static ResolvedPresence ResolveLegacy(
        DiscordStatusUpdater updater,
        string gameName,
        IReadOnlyDictionary<string, object> placeholders,
        string state)
    {
        var mutable = placeholders as Dictionary<string, object> ?? placeholders.ToDictionary(static pair => pair.Key, static pair => pair.Value);
        return new ResolvedPresence(
            updater.UpdateDetails(gameName, mutable, state),
            updater.UpdateState(gameName, mutable, state),
            updater.UpdateLargeAsset(gameName, mutable, state),
            updater.UpdateLargeAssetText(gameName, mutable, state),
            updater.UpdateSmallAsset(gameName, mutable, state),
            updater.UpdateSmallAssetText(gameName, mutable, state),
            updater.UpdateButton1Text(gameName, mutable, state),
            updater.UpdateButton1URL(gameName, mutable, state),
            updater.UpdateButton2Text(gameName, mutable, state),
            updater.UpdateButton2URL(gameName, mutable, state));
    }
}

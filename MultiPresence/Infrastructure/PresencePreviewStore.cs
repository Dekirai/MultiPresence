using System.Collections.ObjectModel;

namespace MultiPresence.Infrastructure;

internal static class PresencePreviewStore
{
    private static readonly object Sync = new();
    private static string game = "";
    private static string state = "";
    private static DateTime updated;
    private static IReadOnlyDictionary<string, object> values = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());

    public static void Capture(string gameName, string gameState, IReadOnlyDictionary<string, object> placeholders)
    {
        var copy = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>(placeholders, StringComparer.OrdinalIgnoreCase));
        lock (Sync)
        {
            game = gameName;
            state = gameState;
            values = copy;
            updated = DateTime.UtcNow;
        }
    }

    public static bool TryGet(string gameName, string gameState, out IReadOnlyDictionary<string, object> placeholders)
    {
        lock (Sync)
        {
            if (game.Equals(gameName, StringComparison.OrdinalIgnoreCase) && state.Equals(gameState, StringComparison.OrdinalIgnoreCase) &&
                DateTime.UtcNow - updated <= TimeSpan.FromSeconds(15))
            {
                placeholders = values;
                return true;
            }
            placeholders = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
            return false;
        }
    }

    public static void Clear()
    {
        lock (Sync)
        {
            game = state = "";
            updated = DateTime.MinValue;
            values = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
        }
    }
}

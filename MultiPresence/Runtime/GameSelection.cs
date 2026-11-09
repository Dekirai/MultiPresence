namespace MultiPresence.Runtime;

internal static class GameSelection
{
    public static DetectedGame? Choose(IEnumerable<DetectedGame> games, string? manualGameName,
        string? preferredGameName, string? currentGameName, Func<string, bool> isBlocked)
    {
        var available = games.Where(game => !isBlocked(game.GameName)).ToArray();
        foreach (var name in new[] { manualGameName, preferredGameName, currentGameName })
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            var selected = available.FirstOrDefault(game => game.GameName.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (selected != null)
            {
                return selected;
            }
        }

        return available.FirstOrDefault();
    }
}

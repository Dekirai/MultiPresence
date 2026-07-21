namespace MultiPresence.Runtime;

internal sealed class BlacklistStore
{
    private readonly string _path;
    private readonly Lock _sync = new();

    public BlacklistStore(string path)
    {
        _path = Path.GetFullPath(path, AppContext.BaseDirectory);
    }

    public bool Contains(string gameName) => Load().Contains(gameName);

    public void SetBlocked(string gameName, bool blocked)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameName);
        lock (_sync)
        {
            var games = Load();
            if (blocked)
            {
                games.Add(gameName);
            }
            else
            {
                games.Remove(gameName);
            }

            var directory = Path.GetDirectoryName(_path)!;
            Directory.CreateDirectory(directory);
            var temporaryPath = _path + ".tmp";
            File.WriteAllLines(temporaryPath, games.Order(StringComparer.OrdinalIgnoreCase));
            File.Move(temporaryPath, _path, true);
        }
    }

    private HashSet<string> Load()
    {
        try
        {
            return File.Exists(_path)
                ? File.ReadAllLines(_path)
                    .Where(static line => !string.IsNullOrWhiteSpace(line))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
        catch (IOException)
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}

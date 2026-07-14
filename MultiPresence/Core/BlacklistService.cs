using System.Text.Json;

namespace MultiPresence.Core;

public sealed class BlacklistService
{
    private readonly string _path;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public BlacklistService(string path)
    {
        _path = Path.GetFullPath(path);
    }

    public async Task<bool> ContainsAsync(string game, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(game))
            return false;

        var games = await LoadAsync(cancellationToken).ConfigureAwait(false);
        return games.Contains(game);
    }

    public async Task SetAsync(string game, bool blocked, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(game))
            return;

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var games = await LoadCoreAsync(cancellationToken).ConfigureAwait(false);
            if (blocked)
                games.Add(game);
            else
                games.Remove(game);

            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var model = new BlacklistModel { Games = games.Order(StringComparer.OrdinalIgnoreCase).ToArray() };
            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
            var tempPath = _path + ".tmp";
            await File.WriteAllTextAsync(tempPath, json, cancellationToken).ConfigureAwait(false);
            File.Move(tempPath, _path, true);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlySet<string>> LoadAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await LoadCoreAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<HashSet<string>> LoadCoreAsync(CancellationToken cancellationToken)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (File.Exists(_path))
        {
            try
            {
                await using var stream = File.OpenRead(_path);
                var model = await JsonSerializer.DeserializeAsync<BlacklistModel>(stream, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (model?.Games is not null)
                {
                    foreach (var game in model.Games.Where(static x => !string.IsNullOrWhiteSpace(x)))
                        result.Add(game.Trim());
                }

                return result;
            }
            catch (JsonException)
            {
                // Fall through to legacy migration.
            }
        }

        var legacyPath = Path.Combine(Path.GetDirectoryName(_path) ?? string.Empty, "blacklist.txt");
        if (File.Exists(legacyPath))
        {
            foreach (var game in await File.ReadAllLinesAsync(legacyPath, cancellationToken).ConfigureAwait(false))
            {
                if (!string.IsNullOrWhiteSpace(game))
                    result.Add(game.Trim());
            }
        }

        return result;
    }

    private sealed class BlacklistModel
    {
        public string[] Games { get; set; } = [];
    }
}

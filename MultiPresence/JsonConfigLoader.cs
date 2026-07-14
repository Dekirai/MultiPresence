using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace MultiPresence
{
    public class ConfigLoader
    {
        private sealed record CacheEntry(DateTime LastWriteUtc, JObject Config);

        private static readonly ConcurrentDictionary<string, CacheEntry> Cache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string, object> PathLocks = new(StringComparer.OrdinalIgnoreCase);

        private readonly JObject _config;

        public ConfigLoader(string configPath)
        {
            _config = LoadConfig(configPath);
        }

        private static JObject LoadConfig(string configPath)
        {
            var absolutePath = Path.GetFullPath(configPath);
            if (!File.Exists(absolutePath))
            {
                Console.WriteLine($"Configuration file '{absolutePath}' not found.");
                return new JObject();
            }

            var lastWriteUtc = File.GetLastWriteTimeUtc(absolutePath);
            if (Cache.TryGetValue(absolutePath, out var cached) && cached.LastWriteUtc == lastWriteUtc)
                return cached.Config;

            var pathLock = PathLocks.GetOrAdd(absolutePath, static _ => new object());
            lock (pathLock)
            {
                lastWriteUtc = File.GetLastWriteTimeUtc(absolutePath);
                if (Cache.TryGetValue(absolutePath, out cached) && cached.LastWriteUtc == lastWriteUtc)
                    return cached.Config;

                var json = File.ReadAllText(absolutePath);
                var parsed = JObject.Parse(json);
                Cache[absolutePath] = new CacheEntry(lastWriteUtc, parsed);
                return parsed;
            }
        }

        public JObject GetConfig() => _config;
    }
}

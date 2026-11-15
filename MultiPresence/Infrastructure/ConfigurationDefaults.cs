using System.Text.Json;

namespace MultiPresence.Infrastructure
{
    internal static class ConfigurationDefaults
    {
        private const string resourcePrefix = "MultiPresence.DefaultConfig.";
        private static readonly Dictionary<string, string> resources = typeof(ConfigurationDefaults).Assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(resourcePrefix, StringComparison.Ordinal))
            .ToDictionary(name => name[resourcePrefix.Length..].Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar),
                name => name, StringComparer.OrdinalIgnoreCase);
        private static readonly Lazy<HashSet<string>> templateGames = new(GetTemplateGames);

        public static bool HasFile(string relativePath) => resources.ContainsKey(relativePath);

        public static bool HasGame(string name) => templateGames.Value.Contains(name);

        private static HashSet<string> GetTemplateGames()
        {
            var games = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string resource in resources.Values)
            {
                using var stream = typeof(ConfigurationDefaults).Assembly.GetManifestResourceStream(resource)!;
                using var document = JsonDocument.Parse(stream);
                foreach (var game in document.RootElement.GetProperty("Games").EnumerateObject()) games.Add(game.Name);
            }
            return games;
        }

        public static void CreateMissingFiles(string baseDirectory)
        {
            var assembly = typeof(ConfigurationDefaults).Assembly;
            var configDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "Assets", "Config"));
            foreach (var (name, resource) in resources)
            {
                string? temporaryPath = null;
                try
                {
                    var configPath = Path.GetFullPath(Path.Combine(configDirectory, name));
                    if (!configPath.StartsWith(configDirectory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                        File.Exists(configPath))
                    {
                        continue;
                    }

                    using var source = assembly.GetManifestResourceStream(resource);
                    if (source == null)
                    {
                        continue;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
                    temporaryPath = configPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
                    using (var destination = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        source.CopyTo(destination);
                    }

                    try
                    {
                        File.Move(temporaryPath, configPath, overwrite: false);
                    }
                    catch (IOException) when (File.Exists(configPath))
                    {
                    }
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                {
                    AppLog.Warning($"Could not create the missing presence configuration '{resource[resourcePrefix.Length..]}'.", exception);
                }
                finally
                {
                    if (temporaryPath != null)
                    {
                        try
                        {
                            File.Delete(temporaryPath);
                        }
                        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                        {
                            AppLog.Warning("Could not remove a temporary configuration file.", exception);
                        }
                    }
                }
            }
        }
    }
}

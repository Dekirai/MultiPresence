namespace MultiPresence.Infrastructure
{
    internal static class ConfigurationDefaults
    {
        private const string resourcePrefix = "MultiPresence.DefaultConfig.";

        public static void CreateMissingFiles(string baseDirectory)
        {
            var assembly = typeof(ConfigurationDefaults).Assembly;
            var configDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "Assets", "Config"));
            foreach (var resource in assembly.GetManifestResourceNames().Where(name => name.StartsWith(resourcePrefix, StringComparison.Ordinal)))
            {
                string? temporaryPath = null;
                try
                {
                    var name = resource[resourcePrefix.Length..].Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
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

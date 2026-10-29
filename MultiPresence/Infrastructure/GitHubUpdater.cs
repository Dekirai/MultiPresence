using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;

namespace MultiPresence.Infrastructure
{
    internal sealed record UpdateAsset(string Name, Uri DownloadUrl, long Size, string? Sha256);

    internal sealed record UpdateRelease(string Tag, UpdateAsset Package, UpdateAsset Updater);

    internal static class GitHubUpdater
    {
        public static UpdateRelease? GetRelease(JsonElement release, string currentReleaseDate)
        {
            if (release.GetProperty("draft").GetBoolean() || release.GetProperty("prerelease").GetBoolean())
            {
                return null;
            }

            var tag = release.GetProperty("tag_name").GetString()?.TrimStart('v');
            if (!DateTime.TryParseExact(tag, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var latest) ||
                !DateTime.TryParseExact(currentReleaseDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var current) ||
                latest <= current)
            {
                return null;
            }

            var assets = release.GetProperty("assets");
            var packageName = assets.EnumerateArray().Any(asset =>
                string.Equals(asset.GetProperty("name").GetString(), "MultiPresence.zip", StringComparison.OrdinalIgnoreCase))
                ? "MultiPresence.zip" : "MultiPresence.exe";
            return new UpdateRelease(latest.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                GetAsset(assets, packageName), GetAsset(assets, "Updater.exe"));
        }

        public static async Task<bool> IsInstalledAsync(UpdateRelease release, string executablePath, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!release.Package.Name.Equals("MultiPresence.exe", StringComparison.OrdinalIgnoreCase) || release.Package.Sha256 == null)
            {
                return false;
            }

            await using var executable = new FileStream(executablePath, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, useAsync: true);
            if (executable.Length != release.Package.Size)
            {
                return false;
            }

            var digest = Convert.ToHexString(await SHA256.HashDataAsync(executable, cancellationToken).ConfigureAwait(false));
            return digest.Equals(release.Package.Sha256, StringComparison.OrdinalIgnoreCase);
        }

        public static async Task<DownloadedUpdate> DownloadAsync(HttpClient client, UpdateRelease release, CancellationToken cancellationToken)
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(client.Timeout);
            cancellationToken = timeout.Token;
            var directory = Path.Combine(Path.GetTempPath(), "MultiPresence", "Updates", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            var update = new DownloadedUpdate(directory);
            try
            {
                var packagePath = Path.Combine(directory, release.Package.Name);
                await DownloadAssetAsync(client, release.Package, packagePath, cancellationToken).ConfigureAwait(false);
                if (release.Package.Name.Equals("MultiPresence.exe", StringComparison.OrdinalIgnoreCase))
                {
                    await CreatePackageAsync(packagePath, update.ArchivePath, cancellationToken).ConfigureAwait(false);
                    File.Delete(packagePath);
                }
                await DownloadAssetAsync(client, release.Updater, update.UpdaterPath, cancellationToken).ConfigureAwait(false);
                ValidatePackage(update.ArchivePath);
                using var executable = File.OpenRead(update.UpdaterPath);
                if (executable.ReadByte() != 'M' || executable.ReadByte() != 'Z')
                {
                    throw new InvalidDataException("The downloaded updater is not a Windows executable.");
                }

                cancellationToken.ThrowIfCancellationRequested();
                return update;
            }
            catch
            {
                update.Dispose();
                throw;
            }
        }

        private static UpdateAsset GetAsset(JsonElement assets, string name)
        {
            var matches = assets.EnumerateArray().Where(asset =>
                string.Equals(asset.GetProperty("name").GetString(), name, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (matches.Length != 1)
            {
                throw new InvalidDataException($"The release must contain exactly one {name} asset.");
            }

            var asset = matches[0];
            if (!Uri.TryCreate(asset.GetProperty("browser_download_url").GetString(), UriKind.Absolute, out var url) ||
                url.Scheme != Uri.UriSchemeHttps || !url.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase) ||
                !url.IsDefaultPort || url.UserInfo.Length != 0 ||
                !url.AbsolutePath.StartsWith("/Dekirai/MultiPresence/releases/download/", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException($"The download address for {name} is invalid.");
            }

            var size = asset.GetProperty("size").GetInt64();
            if (size <= 0)
            {
                throw new InvalidDataException($"The release asset {name} is empty.");
            }

            string? digest = null;
            if (asset.TryGetProperty("digest", out var value) && value.ValueKind == JsonValueKind.String)
            {
                var text = value.GetString();
                if (text != null)
                {
                    if (!text.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase) || text.Length != 71 ||
                        !text.AsSpan(7).ToArray().All(Uri.IsHexDigit))
                    {
                        throw new InvalidDataException($"The checksum for {name} is invalid.");
                    }

                    digest = text[7..];
                }
            }

            return new UpdateAsset(name, url, size, digest);
        }

        private static async Task DownloadAssetAsync(HttpClient client, UpdateAsset asset, string path, CancellationToken cancellationToken)
        {
            using var response = await client.GetAsync(asset.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            await using var source = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            await using var destination = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 65536, useAsync: true);
            using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            var buffer = new byte[65536];
            long total = 0;
            int count;
            while ((count = await source.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
            {
                total += count;
                if (total > asset.Size)
                {
                    throw new InvalidDataException($"The download size for {asset.Name} does not match the release.");
                }

                hash.AppendData(buffer, 0, count);
                await destination.WriteAsync(buffer.AsMemory(0, count), cancellationToken).ConfigureAwait(false);
            }

            var digest = Convert.ToHexString(hash.GetHashAndReset());
            if (total != asset.Size || asset.Sha256 != null && !digest.Equals(asset.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException($"The download for {asset.Name} is incomplete or its checksum does not match.");
            }
        }

        private static async Task CreatePackageAsync(string executablePath, string archivePath, CancellationToken cancellationToken)
        {
            ValidateExecutable(executablePath);
            using var archive = ZipFile.Open(archivePath, ZipArchiveMode.Create);
            var entry = archive.CreateEntry("MultiPresence.exe", CompressionLevel.NoCompression);
            await using var source = new FileStream(executablePath, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, useAsync: true);
            await using var destination = entry.Open();
            await source.CopyToAsync(destination, cancellationToken).ConfigureAwait(false);
        }

        private static void ValidateExecutable(string path)
        {
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream);
            if (stream.Length < 64 || reader.ReadUInt16() != 0x5A4D)
            {
                throw new InvalidDataException("The downloaded executable is invalid.");
            }

            stream.Position = 0x3C;
            var header = reader.ReadInt32();
            if (header < 64 || header > stream.Length - 24)
            {
                throw new InvalidDataException("The downloaded executable is incomplete.");
            }
            stream.Position = header;
            if (reader.ReadUInt32() != 0x4550)
            {
                throw new InvalidDataException("The downloaded executable is invalid.");
            }
        }

        private static void ValidatePackage(string path)
        {
            using var archive = ZipFile.OpenRead(path);
            var executables = archive.Entries.Where(entry => entry.FullName.Equals("MultiPresence.exe", StringComparison.OrdinalIgnoreCase)).ToArray();
            if (executables.Length != 1)
            {
                throw new InvalidDataException("The update package must contain MultiPresence.exe in its root directory.");
            }

            using var executable = executables[0].Open();
            if (executable.ReadByte() != 'M' || executable.ReadByte() != 'Z')
            {
                throw new InvalidDataException("The update package does not contain a Windows executable.");
            }
        }
    }

    internal sealed class DownloadedUpdate : IDisposable
    {
        private bool handedOff;

        public string DirectoryPath { get; }
        public string ArchivePath => Path.Combine(DirectoryPath, "MultiPresence.zip");
        public string UpdaterPath => Path.Combine(DirectoryPath, "Updater.exe");

        public DownloadedUpdate(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        public ProcessStartInfo CreateStartInfo(string executablePath, int processId, long processStart)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = UpdaterPath,
                WorkingDirectory = DirectoryPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            startInfo.ArgumentList.Add(ArchivePath);
            startInfo.ArgumentList.Add(executablePath);
            startInfo.ArgumentList.Add(UpdaterPath);
            startInfo.ArgumentList.Add(processId.ToString(CultureInfo.InvariantCulture));
            startInfo.ArgumentList.Add(processStart.ToString(CultureInfo.InvariantCulture));
            return startInfo;
        }

        public void Launch(string executablePath)
        {
            using var current = Process.GetCurrentProcess();
            using var updater = Process.Start(CreateStartInfo(executablePath, current.Id, current.StartTime.ToUniversalTime().Ticks))
                ?? throw new InvalidOperationException("The updater could not be started.");
            handedOff = true;
        }

        public void Dispose()
        {
            if (handedOff)
            {
                return;
            }

            try
            {
                var root = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "MultiPresence", "Updates")) + Path.DirectorySeparatorChar;
                var path = Path.GetFullPath(DirectoryPath);
                if (path.StartsWith(root, StringComparison.OrdinalIgnoreCase) &&
                    Guid.TryParseExact(Path.GetFileName(path), "N", out _))
                {
                    Directory.Delete(path, recursive: true);
                }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                AppLog.Warning("Could not remove temporary update downloads.", exception);
            }
        }
    }
}

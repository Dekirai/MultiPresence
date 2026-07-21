using MultiPresence.Infrastructure;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MultiPresence;

internal static class FileChecker
{
    private static readonly string[] RequiredFiles = ["MultiPresenceGame.exe", "steam_api64.dll"];
    private static readonly SemaphoreSlim CheckGate = new(1, 1);
    private static readonly HttpClient Client = CreateClient();

    public static async Task EnsureFilesExistAsync()
    {
        await CheckGate.WaitAsync().ConfigureAwait(true);
        try
        {
            var assetsDirectory = Path.Combine(AppContext.BaseDirectory, "Assets");
            var missingFiles = RequiredFiles
                .Where(file => !File.Exists(Path.Combine(assetsDirectory, file)))
                .ToArray();
            if (missingFiles.Length == 0)
            {
                return;
            }

            var answer = MessageBox.Show(
                "This game uses the optional Steam presence host. Download the missing signed release files now?",
                "MultiPresence",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (answer != DialogResult.Yes)
            {
                return;
            }

            Directory.CreateDirectory(assetsDirectory);
            var downloadUrl = await ResolveReleaseArchiveAsync().ConfigureAwait(false);
            var temporaryArchive = Path.Combine(
                Path.GetTempPath(),
                $"MultiPresence-{Guid.NewGuid():N}.zip");

            try
            {
                await using (var destination = new FileStream(
                                 temporaryArchive,
                                 FileMode.CreateNew,
                                 FileAccess.Write,
                                 FileShare.None,
                                 81920,
                                 FileOptions.Asynchronous | FileOptions.SequentialScan))
                await using (var source = await Client.GetStreamAsync(downloadUrl).ConfigureAwait(false))
                {
                    await source.CopyToAsync(destination).ConfigureAwait(false);
                }

                ExtractRequiredFiles(temporaryArchive, assetsDirectory, missingFiles);
                var stillMissing = RequiredFiles
                    .Where(file => !File.Exists(Path.Combine(assetsDirectory, file)))
                    .ToArray();
                if (stillMissing.Length > 0)
                {
                    throw new InvalidDataException(
                        $"The release archive did not contain: {string.Join(", ", stillMissing)}");
                }

                AppLog.Information("Optional Steam presence host files were installed.");
            }
            finally
            {
                try
                {
                    File.Delete(temporaryArchive);
                }
                catch (IOException)
                {
                    // The operating system can clean a locked temporary file later.
                }
            }
        }
        catch (Exception exception) when (
            exception is HttpRequestException or IOException or JsonException or UnauthorizedAccessException)
        {
            AppLog.Warning("Could not install the optional Steam presence host.", exception);
            MessageBox.Show(
                exception.Message,
                "MultiPresence",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            CheckGate.Release();
        }
    }

    private static async Task<Uri> ResolveReleaseArchiveAsync()
    {
        using var response = await Client.GetAsync(
            "https://api.github.com/repos/Dekirai/MultiPresence/releases/latest").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream).ConfigureAwait(false);

        foreach (var asset in document.RootElement.GetProperty("assets").EnumerateArray())
        {
            if (!string.Equals(
                    asset.GetProperty("name").GetString(),
                    "MultiPresence.zip",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var value = asset.GetProperty("browser_download_url").GetString();
            if (Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                return uri;
            }
        }

        throw new InvalidDataException("The latest release does not contain MultiPresence.zip.");
    }

    private static void ExtractRequiredFiles(
        string archivePath,
        string destinationDirectory,
        IReadOnlyCollection<string> missingFiles)
    {
        using var archive = ZipFile.OpenRead(archivePath);
        foreach (var requiredFile in missingFiles)
        {
            var entry = archive.Entries.FirstOrDefault(candidate =>
                string.Equals(candidate.Name, requiredFile, StringComparison.OrdinalIgnoreCase));
            if (entry is null)
            {
                continue;
            }

            // Entry.Name intentionally strips directories and prevents path traversal.
            entry.ExtractToFile(Path.Combine(destinationDirectory, requiredFile), overwrite: true);
        }
    }

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MultiPresence", "12.0"));
        return client;
    }
}

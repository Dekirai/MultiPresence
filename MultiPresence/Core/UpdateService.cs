using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;

namespace MultiPresence.Core;

public sealed class UpdateService
{
    private const string Repository = "Dekirai/MultiPresence";
    private const string CurrentVersion = "11.05.2026";
    private static readonly HttpClient HttpClient = CreateClient();

    public async Task CheckForUpdateAsync(Action<string> notify, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await HttpClient.GetAsync(
                $"https://api.github.com/repos/{Repository}/releases/latest",
                cancellationToken).ConfigureAwait(true);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(true);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(true);
            var release = document.RootElement;

            var latestVersion = release.GetProperty("tag_name").GetString()?.TrimStart('v');
            if (string.IsNullOrWhiteSpace(latestVersion) || !IsNewerVersion(latestVersion, CurrentVersion))
                return;

            var releaseUrl = release.GetProperty("html_url").GetString();
            var assets = release.GetProperty("assets");
            var updaterUrl = FindAssetUrl(assets, "Updater.exe");
            var updateUrl = FindAssetUrl(assets, "MultiPresence.zip");
            var checksumUrl = FindAssetUrl(assets, "MultiPresence.zip.sha256");

            if (updaterUrl is null || updateUrl is null || checksumUrl is null)
            {
                MessageBox.Show(
                    "The release is missing Updater.exe, MultiPresence.zip or MultiPresence.zip.sha256.",
                    "Update Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show(
                $"A new version ({latestVersion}) is available. Do you want to update?",
                "MultiPresence - Update available",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification);

            if (result != DialogResult.Yes)
                return;

            var tempUpdaterPath = Path.Combine(Path.GetTempPath(), $"MultiPresence-Updater-{Guid.NewGuid():N}.exe");
            var tempZipPath = Path.Combine(Path.GetTempPath(), $"MultiPresence-{Guid.NewGuid():N}.zip");

            await DownloadFileAsync(updaterUrl, tempUpdaterPath, cancellationToken).ConfigureAwait(true);
            await DownloadFileAsync(updateUrl, tempZipPath, cancellationToken).ConfigureAwait(true);
            var expectedSha256 = (await HttpClient.GetStringAsync(checksumUrl, cancellationToken).ConfigureAwait(true))
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(expectedSha256))
                throw new InvalidDataException("The release checksum file is empty or invalid.");

            VerifySha256(tempZipPath, expectedSha256);
            StopProcesses("MultiPresenceGame");

            if (MessageBox.Show(
                    "Do you want to view the changelogs?",
                    "MultiPresence - Update available",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.ServiceNotification) == DialogResult.Yes &&
                !string.IsNullOrWhiteSpace(releaseUrl))
            {
                Process.Start(new ProcessStartInfo(releaseUrl) { UseShellExecute = true });
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = tempUpdaterPath,
                Arguments = $"\"{tempZipPath}\" \"{Application.ExecutablePath}\" \"{tempUpdaterPath}\" \"{expectedSha256}\"",
                UseShellExecute = true
            });

            notify("Update downloaded, MultiPresence is now restarting and updating files!");
            Environment.Exit(0);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            RateLimitedLogger.Error("update-check", ex);
            notify("Failed to update MultiPresence!");
            MessageBox.Show($"Error checking for updates: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MultiPresence", CurrentVersion.Replace('.', '-')));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        return client;
    }

    private static string? FindAssetUrl(JsonElement assets, string assetName)
    {
        foreach (var asset in assets.EnumerateArray())
        {
            if (asset.GetProperty("name").GetString()?.Equals(assetName, StringComparison.OrdinalIgnoreCase) == true)
                return asset.GetProperty("browser_download_url").GetString();
        }

        return null;
    }

    private static async Task DownloadFileAsync(string url, string destination, CancellationToken cancellationToken)
    {
        using var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        await using var source = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        await using var destinationStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await source.CopyToAsync(destinationStream, cancellationToken).ConfigureAwait(false);
    }

    private static void VerifySha256(string filePath, string expectedSha256)
    {
        using var stream = File.OpenRead(filePath);
        var actualSha256 = Convert.ToHexString(SHA256.HashData(stream));
        if (!actualSha256.Equals(expectedSha256.Trim(), StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Downloaded update archive SHA-256 does not match the release checksum.");
    }

    private static bool IsNewerVersion(string latest, string current)
        => DateTime.TryParseExact(latest, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var latestDate)
           && DateTime.TryParseExact(current, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var currentDate)
           && latestDate > currentDate;

    private static void StopProcesses(string processName)
    {
        foreach (var process in Process.GetProcessesByName(processName))
        {
            using (process)
            {
                try
                {
                    process.Kill();
                    process.WaitForExit(5000);
                }
                catch (Exception ex)
                {
                    RateLimitedLogger.Error($"stop-process:{processName}", ex);
                }
            }
        }
    }
}

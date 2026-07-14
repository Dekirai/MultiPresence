using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;

internal static class Updater
{
    private static int Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: Updater.exe <update.zip> <mainExecutablePath> <updaterPath> [expectedSha256]");
            return 2;
        }

        var updateZipPath = Path.GetFullPath(args[0]);
        var mainExecutablePath = Path.GetFullPath(args[1]);
        var updaterPath = Path.GetFullPath(args[2]);
        var expectedSha256 = args.Length >= 4 ? args[3] : null;
        var appDirectory = Path.GetDirectoryName(mainExecutablePath)
            ?? throw new InvalidOperationException("Could not resolve application directory.");
        var stagingDirectory = Path.Combine(Path.GetTempPath(), $"MultiPresence-update-{Guid.NewGuid():N}");
        var backupDirectory = Path.Combine(Path.GetTempPath(), $"MultiPresence-backup-{Guid.NewGuid():N}");

        try
        {
            WaitForProcessToExit("MultiPresence", TimeSpan.FromMinutes(2));
            WaitForProcessToExit("MultiPresenceGame", TimeSpan.FromMinutes(2));

            ValidateArchive(updateZipPath, expectedSha256);
            Directory.CreateDirectory(stagingDirectory);
            ExtractZipSafely(updateZipPath, stagingDirectory);
            ValidateStagedUpdate(stagingDirectory);

            Directory.CreateDirectory(backupDirectory);
            BackupInstallation(appDirectory, backupDirectory);

            try
            {
                CopyDirectory(stagingDirectory, appDirectory, overwrite: true);
                if (!File.Exists(mainExecutablePath))
                    throw new InvalidDataException("Updated MultiPresence executable is missing after installation.");
            }
            catch
            {
                RestoreBackup(backupDirectory, appDirectory);
                throw;
            }

            File.Delete(updateZipPath);
            Process.Start(new ProcessStartInfo(mainExecutablePath) { UseShellExecute = true });
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Update failed: {ex}");
            return 1;
        }
        finally
        {
            TryDeleteDirectory(stagingDirectory);
            TryDeleteDirectory(backupDirectory);
            ScheduleSelfDelete(updaterPath);
        }
    }

    private static void WaitForProcessToExit(string processName, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            var processes = Process.GetProcessesByName(processName);
            try
            {
                if (processes.Length == 0)
                    return;
            }
            finally
            {
                foreach (var process in processes)
                    process.Dispose();
            }

            Thread.Sleep(500);
        }

        throw new TimeoutException($"Timed out waiting for {processName} to exit.");
    }

    private static void ValidateArchive(string zipPath, string? expectedSha256)
    {
        if (!File.Exists(zipPath))
            throw new FileNotFoundException("Update archive was not found.", zipPath);

        if (new FileInfo(zipPath).Length == 0)
            throw new InvalidDataException("Update archive is empty.");

        if (!string.IsNullOrWhiteSpace(expectedSha256))
        {
            using var stream = File.OpenRead(zipPath);
            var actual = Convert.ToHexString(SHA256.HashData(stream));
            if (!actual.Equals(expectedSha256.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Update archive SHA-256 does not match the release checksum.");
        }

        using var archive = ZipFile.OpenRead(zipPath);
        if (archive.Entries.Count == 0)
            throw new InvalidDataException("Update archive contains no files.");
    }

    private static void ExtractZipSafely(string zipPath, string extractPath)
    {
        var root = Path.GetFullPath(extractPath) + Path.DirectorySeparatorChar;
        using var archive = ZipFile.OpenRead(zipPath);

        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
                continue;

            var destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));
            if (!destinationPath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException($"Archive entry escapes update directory: {entry.FullName}");

            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
            entry.ExtractToFile(destinationPath, overwrite: true);
        }
    }

    private static void ValidateStagedUpdate(string stagingDirectory)
    {
        var requiredFiles = new[] { "MultiPresence.exe", "MultiPresence.dll" };
        foreach (var file in requiredFiles)
        {
            if (!File.Exists(Path.Combine(stagingDirectory, file)))
                throw new InvalidDataException($"Update is missing required file '{file}'.");
        }
    }

    private static void BackupInstallation(string sourceDirectory, string backupDirectory)
    {
        foreach (var file in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceDirectory, file);
            if (relative.StartsWith("Assets\\Config", StringComparison.OrdinalIgnoreCase) ||
                relative.Equals("Assets\\blacklist.json", StringComparison.OrdinalIgnoreCase))
                continue;

            var destination = Path.Combine(backupDirectory, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(file, destination, true);
        }
    }

    private static void RestoreBackup(string backupDirectory, string appDirectory)
    {
        CopyDirectory(backupDirectory, appDirectory, overwrite: true);
    }

    private static void CopyDirectory(string sourceDirectory, string destinationDirectory, bool overwrite)
    {
        foreach (var directory in Directory.EnumerateDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceDirectory, directory);
            Directory.CreateDirectory(Path.Combine(destinationDirectory, relative));
        }

        foreach (var file in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceDirectory, file);
            var destination = Path.Combine(destinationDirectory, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(file, destination, overwrite);
        }
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
        catch
        {
        }
    }

    private static void ScheduleSelfDelete(string updaterPath)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C timeout /t 2 /nobreak >nul & del /f /q \"{updaterPath}\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }
        catch
        {
        }
    }
}

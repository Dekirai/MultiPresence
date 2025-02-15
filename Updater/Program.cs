using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

class Updater
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: Updater.exe <update.zip> <mainExecutablePath> <updaterPath>");
            return;
        }

        string updateZipPath = args[0];
        string mainExecutablePath = args[1];
        string updaterPath = args[2];
        string appDirectory = Path.GetDirectoryName(mainExecutablePath);

        WaitForProcessToExit("MultiPresence");
        WaitForProcessToExit("MultiPresenceGame");

        try
        {
            DeleteFile(mainExecutablePath);
            DeleteFile(Path.Combine(appDirectory, "MultiPresenceGame.exe"));

            ExtractZip(updateZipPath, appDirectory);

            File.Delete(updateZipPath);

            Process.Start(mainExecutablePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating: {ex.Message}");
        }

        ScheduleSelfDelete(updaterPath);
    }

    private static void WaitForProcessToExit(string processName)
    {
        while (Process.GetProcessesByName(processName).Any())
        {
            Thread.Sleep(1000);
        }
    }

    private static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting {filePath}: {ex.Message}");
            }
        }
    }

    private static void ExtractZip(string zipPath, string extractPath)
    {
        try
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string destinationPath = Path.Combine(extractPath, entry.FullName);

                    if (entry.FullName.EndsWith("/"))
                        continue;

                    string directoryPath = Path.GetDirectoryName(destinationPath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    entry.ExtractToFile(destinationPath, true);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting files: {ex.Message}");
        }
    }

    private static void ScheduleSelfDelete(string updaterPath)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/C timeout /t 2 & del \"{updaterPath}\"",
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true
        });
    }
}

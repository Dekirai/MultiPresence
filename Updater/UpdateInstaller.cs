using System.Diagnostics;
using System.IO.Compression;
using MultiPresence.Infrastructure;

namespace MultiPresence.Updater
{
    internal static class UpdateInstaller
    {
        public static PreparedUpdate Prepare(string zipPath, string executablePath, string workDirectory)
        {
            executablePath = Path.GetFullPath(executablePath);
            var directory = Path.GetDirectoryName(executablePath) ?? throw new InvalidDataException("The installation directory is unavailable.");
            workDirectory = Path.GetFullPath(workDirectory);
            if (!Path.GetFileName(executablePath).Equals("MultiPresence.exe", StringComparison.OrdinalIgnoreCase) ||
                !File.Exists(executablePath) || IsWithin(directory, workDirectory) || IsWithin(workDirectory, directory) ||
                Directory.Exists(workDirectory) || File.Exists(workDirectory))
            {
                throw new InvalidDataException("The update paths are invalid.");
            }

            EnsureSafeTarget(directory, executablePath);
            using var archive = ZipFile.OpenRead(zipPath);
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var files = new List<UpdateFile>();
            var entries = new List<(ZipArchiveEntry Entry, string Name)>();
            long total = 0;
            if (archive.Entries.Count > 10000)
            {
                throw new InvalidDataException("The update contains too many files.");
            }

            foreach (var entry in archive.Entries)
            {
                var name = GetEntryName(entry);
                if (!names.Add(name))
                {
                    throw new InvalidDataException("The update contains duplicate paths.");
                }

                var target = Path.GetFullPath(Path.Combine(directory, name));
                EnsureSafeTarget(directory, target);
                if (entry.FullName.EndsWith('/') || entry.FullName.EndsWith('\\'))
                {
                    if (entry.Length != 0 || File.Exists(target))
                    {
                        throw new InvalidDataException("The update contains an invalid directory.");
                    }
                    continue;
                }

                if (Directory.Exists(target) || entry.Length < 0 || entry.Length > 1024L * 1024 * 1024)
                {
                    throw new InvalidDataException("The update contains an invalid file.");
                }
                total = checked(total + entry.Length);
                if (total > 2L * 1024 * 1024 * 1024)
                {
                    throw new InvalidDataException("The update is too large.");
                }
                entries.Add((entry, name));
            }

            if (!entries.Any(item => item.Name.Equals("MultiPresence.exe", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidDataException("MultiPresence.exe is missing from the update.");
            }

            var fileNames = entries.Select(item => item.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var name in names)
            {
                var parent = Path.GetDirectoryName(name);
                while (!string.IsNullOrEmpty(parent))
                {
                    if (fileNames.Contains(parent))
                    {
                        throw new InvalidDataException("The update contains conflicting paths.");
                    }
                    parent = Path.GetDirectoryName(parent);
                }
            }

            Directory.CreateDirectory(workDirectory);
            foreach (var item in entries)
            {
                var source = Path.Combine(workDirectory, "staged", item.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(source) ?? workDirectory);
                using (var input = item.Entry.Open())
                using (var output = new FileStream(source, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    input.CopyTo(output);
                    if (output.Length != item.Entry.Length)
                    {
                        throw new InvalidDataException("An update file is incomplete.");
                    }
                }

                files.Add(new UpdateFile(item.Name, source, Path.Combine(directory, item.Name), IsUserConfiguration(item.Name)));
            }

            ValidateExecutable(files.Single(file => file.Name.Equals("MultiPresence.exe", StringComparison.OrdinalIgnoreCase)).Source);
            return new PreparedUpdate(executablePath, directory, workDirectory, files);
        }

        private static string GetEntryName(ZipArchiveEntry entry)
        {
            var name = entry.FullName.Replace('\\', '/');
            var kind = (entry.ExternalAttributes >> 16) & 0xF000;
            if (name.Length == 0 || name.StartsWith('/') || kind is not 0 and not 0x8000 and not 0x4000 ||
                (entry.ExternalAttributes & (int)FileAttributes.ReparsePoint) != 0)
            {
                throw new InvalidDataException("The update contains an unsafe path.");
            }

            var parts = name.TrimEnd('/').Split('/');
            foreach (var part in parts)
            {
                var stem = part.Split('.')[0];
                if (part.Length == 0 || part is "." or ".." || part.EndsWith('.') || part.EndsWith(' ') ||
                    part.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                    stem.Equals("CON", StringComparison.OrdinalIgnoreCase) || stem.Equals("PRN", StringComparison.OrdinalIgnoreCase) ||
                    stem.Equals("AUX", StringComparison.OrdinalIgnoreCase) || stem.Equals("NUL", StringComparison.OrdinalIgnoreCase) ||
                    stem.Length == 4 && (stem.StartsWith("COM", StringComparison.OrdinalIgnoreCase) || stem.StartsWith("LPT", StringComparison.OrdinalIgnoreCase)) && char.IsDigit(stem[3]))
                {
                    throw new InvalidDataException("The update contains an unsafe path.");
                }
            }

            return string.Join(Path.DirectorySeparatorChar, parts);
        }

        private static bool IsUserConfiguration(string name)
        {
            name = name.Replace('\\', '/');
            return name.StartsWith("Assets/Config/", StringComparison.OrdinalIgnoreCase) ||
                name.EndsWith(".config", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Assets/blacklist.txt", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("Assets/blacklist.json", StringComparison.OrdinalIgnoreCase);
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

        internal static bool IsWithin(string directory, string path)
        {
            return path.Equals(directory, StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith(Path.TrimEndingDirectorySeparator(directory) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        internal static void EnsureSafeTarget(string directory, string path)
        {
            if (!IsWithin(directory, path))
            {
                throw new InvalidDataException("An update path leaves the installation directory.");
            }

            var current = path;
            while (IsWithin(directory, current))
            {
                try
                {
                    if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                    {
                        throw new InvalidDataException("An update path contains a filesystem link.");
                    }
                }
                catch (FileNotFoundException)
                {
                }
                catch (DirectoryNotFoundException)
                {
                }

                if (current.Equals(directory, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                current = Path.GetDirectoryName(current) ?? throw new InvalidDataException("An update path has no parent.");
            }
        }
    }

    internal sealed record UpdateFile(string Name, string Source, string Target, bool PreserveExisting);

    internal sealed class PreparedUpdate
    {
        private readonly string directory;
        private readonly List<UpdateFile> files;
        private readonly List<(UpdateFile File, string? Backup)> applied = new();
        private bool installed;

        public string ExecutablePath { get; }
        public string WorkDirectory { get; }

        internal PreparedUpdate(string executablePath, string directory, string workDirectory, List<UpdateFile> files)
        {
            ExecutablePath = executablePath;
            WorkDirectory = workDirectory;
            this.directory = directory;
            this.files = files;
        }

        public void Install()
        {
            if (installed || applied.Count != 0)
            {
                throw new InvalidOperationException("The update was already installed.");
            }

            try
            {
                foreach (var file in files.OrderBy(file => file.PreserveExisting))
                {
                    UpdateInstaller.EnsureSafeTarget(directory, file.Target);
                    if (file.PreserveExisting && File.Exists(file.Target))
                    {
                        continue;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(file.Target) ?? directory);
                    var temporary = file.Target + "." + Guid.NewGuid().ToString("N") + ".update";
                    string? backup = null;
                    try
                    {
                        File.Copy(file.Source, temporary, overwrite: false);
                        if (!file.PreserveExisting && File.Exists(file.Target))
                        {
                            backup = Path.Combine(WorkDirectory, "backup", file.Name);
                            Directory.CreateDirectory(Path.GetDirectoryName(backup) ?? WorkDirectory);
                            File.Copy(file.Target, backup, overwrite: false);
                        }

                        UpdateInstaller.EnsureSafeTarget(directory, file.Target);
                        try
                        {
                            if (file.PreserveExisting)
                            {
                                File.Move(temporary, file.Target, overwrite: false);
                            }
                            else
                            {
                                ReplaceFile(temporary, file.Target);
                            }
                        }
                        catch (IOException) when (file.PreserveExisting && File.Exists(file.Target))
                        {
                            continue;
                        }

                        if (!file.PreserveExisting)
                        {
                            applied.Add((file, backup));
                        }
                    }
                    finally
                    {
                        File.Delete(temporary);
                    }
                }

                installed = true;
            }
            catch (Exception installError)
            {
                try
                {
                    Rollback();
                }
                catch (Exception rollbackError)
                {
                    throw new AggregateException("The update failed and some files could not be restored. The backup is in " + WorkDirectory, installError, rollbackError);
                }
                throw;
            }
        }

        public void Rollback()
        {
            var errors = new List<Exception>();
            for (var index = applied.Count - 1; index >= 0; index--)
            {
                var item = applied[index];
                var temporary = item.File.Target + "." + Guid.NewGuid().ToString("N") + ".restore";
                try
                {
                    UpdateInstaller.EnsureSafeTarget(directory, item.File.Target);
                    if (item.Backup == null)
                    {
                        File.Delete(item.File.Target);
                    }
                    else
                    {
                        File.Copy(item.Backup, temporary, overwrite: false);
                        ReplaceFile(temporary, item.File.Target);
                    }
                    applied.RemoveAt(index);
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                {
                    errors.Add(exception);
                }
                finally
                {
                    try
                    {
                        File.Delete(temporary);
                    }
                    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                    {
                        errors.Add(exception);
                    }
                }
            }

            installed = false;
            if (errors.Count > 0)
            {
                throw new AggregateException("The previous version could not be completely restored. The backup is in " + WorkDirectory, errors);
            }
        }

        private void ReplaceFile(string source, string target)
        {
            var timer = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    UpdateInstaller.EnsureSafeTarget(directory, target);
                    File.Move(source, target, overwrite: true);
                    return;
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                {
                    var error = exception.HResult & 0xFFFF;
                    if (error is not 5 and not 32 and not 33 || timer.Elapsed >= TimeSpan.FromSeconds(10) ||
                        Directory.Exists(target) || File.Exists(target) && (File.GetAttributes(target) & FileAttributes.ReadOnly) != 0)
                    {
                        throw new IOException($"Could not replace '{target}'. The file is still in use or access was denied.", exception);
                    }
                    Thread.Sleep(250);
                }
            }
        }

        public void Complete()
        {
            if (!installed)
            {
                throw new InvalidOperationException("The update is not installed.");
            }

            try
            {
                if (!UpdateInstaller.IsWithin(directory, WorkDirectory) && !UpdateInstaller.IsWithin(WorkDirectory, directory))
                {
                    Directory.Delete(WorkDirectory, recursive: true);
                }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                AppLog.Warning("Could not remove the completed update files.", exception);
            }
        }
    }
}

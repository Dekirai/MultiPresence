using System.Text.Json;

namespace MultiPresence.Infrastructure;

internal static class AppLog
{
    private const long MaximumLogSize = 2 * 1024 * 1024;
    private static readonly object Sync = new();
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    private static string LogDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MultiPresence",
        "Logs");

    internal static string CurrentLogPath => Path.Combine(LogDirectory, "multipresence.log");

    public static void Information(string message, object? data = null) =>
        Write("Information", message, null, data);

    public static void Warning(string message, Exception? exception = null, object? data = null) =>
        Write("Warning", message, exception, data);

    public static void Error(string message, Exception exception, object? data = null) =>
        Write("Error", message, exception, data);

    private static void Write(string level, string message, Exception? exception, object? data)
    {
        try
        {
            lock (Sync)
            {
                Directory.CreateDirectory(LogDirectory);
                RotateIfRequired();

                var entry = new
                {
                    Timestamp = DateTimeOffset.Now,
                    Level = level,
                    Message = message,
                    Exception = exception?.ToString(),
                    Data = data
                };

                File.AppendAllText(
                    CurrentLogPath,
                    JsonSerializer.Serialize(entry, JsonOptions) + Environment.NewLine);
            }
        }
        catch
        {
            // Logging must never terminate the application.
        }
    }

    private static void RotateIfRequired()
    {
        var current = new FileInfo(CurrentLogPath);
        if (!current.Exists || current.Length < MaximumLogSize)
        {
            return;
        }

        var archivePath = Path.Combine(LogDirectory, "multipresence.previous.log");
        File.Move(CurrentLogPath, archivePath, true);
    }
}

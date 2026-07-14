using System.Collections.Concurrent;

namespace MultiPresence.Core;

public static class RateLimitedLogger
{
    private static readonly ConcurrentDictionary<string, DateTimeOffset> LastWrite = new(StringComparer.Ordinal);

    public static void Warning(string key, string message, TimeSpan? interval = null)
        => Write("WARN", key, message, interval ?? TimeSpan.FromMinutes(1));

    public static void Error(string key, Exception exception, TimeSpan? interval = null)
        => Write("ERROR", key, exception.ToString(), interval ?? TimeSpan.FromMinutes(1));

    private static void Write(string level, string key, string message, TimeSpan interval)
    {
        var now = DateTimeOffset.UtcNow;
        var last = LastWrite.GetOrAdd(key, DateTimeOffset.MinValue);
        if (now - last < interval)
            return;

        LastWrite[key] = now;
        Trace.WriteLine($"[{now:O}] [{level}] {message}");
    }
}

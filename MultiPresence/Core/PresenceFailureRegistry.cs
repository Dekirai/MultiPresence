using System.Collections.Concurrent;

namespace MultiPresence.Core;

public static class PresenceFailureRegistry
{
    private static readonly ConcurrentDictionary<string, byte> FailedPresenceTypes = new(StringComparer.OrdinalIgnoreCase);

    public static void Report(string presenceType, Exception exception)
    {
        FailedPresenceTypes[presenceType] = 0;
        RateLimitedLogger.Error($"presence-loop:{presenceType}", exception);
    }

    public static bool Consume(string presenceType)
        => FailedPresenceTypes.TryRemove(presenceType, out _);

    public static void Clear(string presenceType)
        => FailedPresenceTypes.TryRemove(presenceType, out _);
}

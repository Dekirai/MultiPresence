namespace MultiPresence.Core;

public sealed class ManagedThread
{
    private readonly Action _action;
    private int _started;

    public ManagedThread(Action action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public void Start()
    {
        if (Interlocked.Exchange(ref _started, 1) != 0)
            throw new ThreadStateException("The managed thread has already been started.");

        _ = Task.Run(RunAsyncVoidCompatible);
    }

    private void RunAsyncVoidCompatible()
    {
        var previousContext = SynchronizationContext.Current;
        try
        {
            SynchronizationContext.SetSynchronizationContext(new LoggingSynchronizationContext());
            _action();
        }
        catch (Exception ex)
        {
            RateLimitedLogger.Error($"managed-thread:{_action.Method.DeclaringType?.FullName}.{_action.Method.Name}", ex);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }

    private sealed class LoggingSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object? state)
        {
            _ = Task.Run(() =>
            {
                try
                {
                    SetSynchronizationContext(this);
                    d(state);
                }
                catch (Exception ex)
                {
                    RateLimitedLogger.Error("legacy-async-void", ex);
                }
            });
        }
    }
}

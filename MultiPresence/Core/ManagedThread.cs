namespace MultiPresence.Core;

public sealed class ManagedThread
{
    private readonly Action _action;
    private readonly string _presenceType;
    private int _started;

    public ManagedThread(Action action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _presenceType = action.Method.DeclaringType?.Name ?? action.Method.Name;
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
            SynchronizationContext.SetSynchronizationContext(new LoggingSynchronizationContext(_presenceType));
            _action();
        }
        catch (Exception ex)
        {
            PresenceFailureRegistry.Report(_presenceType, ex);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(previousContext);
        }
    }

    private sealed class LoggingSynchronizationContext(string presenceType) : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object? state)
        {
            _ = Task.Run(() =>
            {
                var previousContext = Current;
                try
                {
                    SetSynchronizationContext(this);
                    d(state);
                }
                catch (Exception ex)
                {
                    PresenceFailureRegistry.Report(presenceType, ex);
                }
                finally
                {
                    SetSynchronizationContext(previousContext);
                }
            });
        }
    }
}

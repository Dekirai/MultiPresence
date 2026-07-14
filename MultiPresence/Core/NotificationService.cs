namespace MultiPresence.Core;

public sealed class NotificationService
{
    private readonly NotifyIcon _notifyIcon;
    private readonly Func<bool> _notificationsDisabled;

    public NotificationService(NotifyIcon notifyIcon, Func<bool> notificationsDisabled)
    {
        _notifyIcon = notifyIcon ?? throw new ArgumentNullException(nameof(notifyIcon));
        _notificationsDisabled = notificationsDisabled ?? throw new ArgumentNullException(nameof(notificationsDisabled));
    }

    public void ShowGameTracking(string game)
    {
        if (_notificationsDisabled())
            return;

        Show("System", $"Keeping track of {game}.");
    }

    public void ShowUpdateStatus(string message)
        => Show("MultiPresence - Update status", message);

    private void Show(string title, string message)
    {
        _notifyIcon.BalloonTipTitle = title;
        _notifyIcon.BalloonTipText = message;
        _notifyIcon.ShowBalloonTip(3000);
    }
}

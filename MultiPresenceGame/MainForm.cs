using MultiPresenceGame.Presence;

namespace MultiPresenceGame;

public partial class MainForm : Form
{
    private static readonly IReadOnlyDictionary<string, Action> Integrations =
        new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
        {
            ["Call of Duty®"] = COD.DoAction,
            ["Diablo IV"] = D4.DoAction,
            ["Gunfire Reborn"] = GFR.DoAction,
            ["Hello Kitty Island Adventure"] = HK.DoAction,
            ["Hogwarts Legacy"] = HL.DoAction,
            ["Labyrinthine"] = LR.DoAction,
            ["Overwatch"] = OW.DoAction,
            ["Team Fortress 2"] = TF2.DoAction,
            ["Temtem: Swarm"] = TTS.DoAction
        };

    private readonly System.Windows.Forms.Timer _detectionTimer = new() { Interval = 2000 };

    public MainForm()
    {
        InitializeComponent();
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        _detectionTimer.Tick += DetectionTimerOnTick;
        _detectionTimer.Start();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        Hide();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _detectionTimer.Stop();
        _detectionTimer.Tick -= DetectionTimerOnTick;
        base.OnFormClosed(e);
    }

    private void DetectionTimerOnTick(object? sender, EventArgs e)
    {
        var game = GameDetector.GetGame();
        if (!Integrations.TryGetValue(game, out var start))
        {
            return;
        }

        _detectionTimer.Stop();
        start();
    }
}

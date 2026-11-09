namespace MultiPresence;

internal sealed class PreferredGameForm : Form
{
    private readonly ComboBox games = new() { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };

    public string SelectedGame => games.SelectedIndex <= 0 ? "" : games.SelectedItem?.ToString() ?? "";

    public PreferredGameForm(IEnumerable<string> supportedGames, string preferredGame)
    {
        Text = "Preferred game — MultiPresence";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(520, 155);
        var layout = new TableLayoutPanel()
        {
            Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 1, RowCount = 3
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(new Label()
        {
            Text = "In Automatic mode, use this game first when several games are running.",
            Dock = DockStyle.Fill
        }, 0, 0);
        games.Items.Add("No preference");
        foreach (var game in supportedGames.Order(StringComparer.OrdinalIgnoreCase))
        {
            games.Items.Add(game);
        }
        games.SelectedIndex = Math.Max(0, games.Items.IndexOf(preferredGame));
        layout.Controls.Add(games, 0, 1);
        var buttons = new FlowLayoutPanel() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var cancel = new Button() { Text = "Cancel", DialogResult = DialogResult.Cancel, AutoSize = true };
        var save = new Button() { Text = "Save", DialogResult = DialogResult.OK, AutoSize = true };
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        layout.Controls.Add(buttons, 0, 2);
        Controls.Add(layout);
        AcceptButton = save;
        CancelButton = cancel;
    }
}

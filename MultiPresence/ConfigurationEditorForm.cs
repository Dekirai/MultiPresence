using MultiPresence.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MultiPresence
{
    internal sealed class ConfigurationEditorForm : Form
    {
        private readonly ComboBox fileSelector = new() { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        private readonly ComboBox gameSelector = new() { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        private readonly ComboBox stateSelector = new() { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        private readonly Button saveButton = new() { Text = "Save", AutoSize = true, Enabled = false };
        private readonly Button reloadButton = new() { Text = "Reload", AutoSize = true };
        private readonly TabControl editorTabs = new() { Dock = DockStyle.Fill };
        private readonly TabPage fieldsPage = new("Fields");
        private readonly TextBox jsonEditor = new() { Multiline = true, AcceptsTab = true, AcceptsReturn = true, WordWrap = false, ScrollBars = ScrollBars.Both, Dock = DockStyle.Fill, Font = new Font("Consolas", 10F), MaxLength = 4 * 1024 * 1024 };
        private readonly Dictionary<string, TextBox> fieldEditors = new();
        private readonly TextBox validation = new() { Multiline = true, ReadOnly = true, BorderStyle = BorderStyle.None, BackColor = SystemColors.Control, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill, TabStop = false };
        private readonly Label previewSource = new() { AutoSize = false, Dock = DockStyle.Top, Height = 34, Text = "Sample values", ForeColor = SystemColors.GrayText };
        private readonly Label previewGame = new() { AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 11F, FontStyle.Bold), MaximumSize = new Size(430, 0) };
        private readonly Label previewDetails = new() { AutoSize = true, ForeColor = Color.White, MaximumSize = new Size(430, 0) };
        private readonly Label previewState = new() { AutoSize = true, ForeColor = Color.Gainsboro, MaximumSize = new Size(430, 0) };
        private readonly TextBox previewAssets = new() { Multiline = true, ReadOnly = true, BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(43, 45, 49), ForeColor = Color.Silver, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical, TabStop = false };
        private readonly TabControl referenceTabs = new() { Dock = DockStyle.Fill };
        private readonly ListView placeholderList = new() { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, MultiSelect = false, HideSelection = false };
        private readonly ListView assetList = new() { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true, MultiSelect = false, HideSelection = false };
        private readonly TextBox placeholderSearch = new() { Dock = DockStyle.Top, PlaceholderText = "Filter placeholders" };
        private readonly TextBox assetSearch = new() { Dock = DockStyle.Top, PlaceholderText = "Filter assets" };
        private readonly System.Windows.Forms.Timer editTimer = new() { Interval = 250 };
        private readonly System.Windows.Forms.Timer previewTimer = new() { Interval = 1000 };
        private ConfigurationEditorDocument? document;
        private TextBox? insertionTarget;
        private IReadOnlyList<EditorPlaceholder> placeholders = Array.Empty<EditorPlaceholder>();
        private IReadOnlyList<EditorAsset> assets = Array.Empty<EditorAsset>();
        private string referenceGame = "";
        private int loadedFileIndex = -1;
        private bool binding;
        private bool valid;

        public ConfigurationEditorForm(string? initialGame = null)
        {
            Text = "MultiPresence — Config editor";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 680);
            var workingArea = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1180, 800);
            Size = new Size(Math.Max(MinimumSize.Width, Math.Min(1180, workingArea.Width - 40)), Math.Max(MinimumSize.Height, Math.Min(800, workingArea.Height - 40)));
            Font = new Font("Segoe UI", 9F);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            BuildLayout();
            fileSelector.SelectedIndexChanged += (_, _) => SelectFile();
            gameSelector.SelectedIndexChanged += (_, _) => { if (!binding) { referenceGame = ""; RefreshView(); } };
            stateSelector.SelectedIndexChanged += (_, _) => { if (!binding) RefreshView(); };
            jsonEditor.TextChanged += (_, _) =>
            {
                if (!binding && document != null)
                {
                    document.Text = jsonEditor.Text;
                    ScheduleRefresh();
                }
            };
            jsonEditor.Enter += (_, _) => insertionTarget = jsonEditor;
            editorTabs.SelectedIndexChanged += (_, _) =>
            {
                if (editorTabs.SelectedIndex == 1) insertionTarget = jsonEditor;
                else if (insertionTarget == jsonEditor) insertionTarget = fieldEditors["Details"];
            };
            saveButton.Click += (_, _) => SaveDocument();
            reloadButton.Click += (_, _) => ReloadDocument();
            placeholderSearch.TextChanged += (_, _) => FilterReferences();
            assetSearch.TextChanged += (_, _) => FilterReferences();
            placeholderList.DoubleClick += (_, _) => InsertPlaceholder();
            assetList.DoubleClick += (_, _) => InsertAsset();
            editTimer.Tick += (_, _) => { editTimer.Stop(); RefreshView(); };
            previewTimer.Tick += (_, _) => { if (valid && document != null) UpdatePreview(); };
            FormClosing += (_, args) => args.Cancel = !ConfirmUnsaved();
            LoadFiles(initialGame);
            previewTimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                editTimer.Dispose();
                previewTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        private void BuildLayout()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 1, RowCount = 4 };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            var selectors = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 2 };
            selectors.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48));
            selectors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            selectors.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48));
            selectors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            selectors.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            selectors.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            selectors.Controls.Add(new Label { Text = "File", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            selectors.Controls.Add(fileSelector, 1, 0);
            selectors.SetColumnSpan(fileSelector, 3);
            selectors.Controls.Add(new Label { Text = "Game", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            selectors.Controls.Add(gameSelector, 1, 1);
            selectors.Controls.Add(new Label { Text = "State", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 2, 1);
            selectors.Controls.Add(stateSelector, 3, 1);
            layout.Controls.Add(selectors, 0, 0);

            var split = new SplitContainer { Size = new Size(1120, 500), Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterWidth = 10, Panel1MinSize = 480, Panel2MinSize = 360, SplitterDistance = 600 };
            var fields = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Padding = new Padding(8) };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (var field in ConfigurationEditorDocument.Fields)
            {
                int row = fields.RowCount++;
                fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
                var editor = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(3, 8, 3, 8), Name = field, Tag = field };
                editor.Enter += (_, _) => insertionTarget = editor;
                editor.TextChanged += (_, _) => FieldChanged(field, editor.Text);
                fieldEditors.Add(field, editor);
                fields.Controls.Add(new Label { Text = FieldLabel(field), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
                fields.Controls.Add(editor, 1, row);
            }
            fieldsPage.AutoScroll = true;
            fieldsPage.Controls.Add(fields);
            var jsonPage = new TabPage("JSON");
            jsonPage.Controls.Add(jsonEditor);
            editorTabs.TabPages.AddRange(new[] { fieldsPage, jsonPage });
            split.Panel1.Controls.Add(editorTabs);

            var right = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 52));
            var preview = new GroupBox { Text = "Presence preview", Dock = DockStyle.Fill, Padding = new Padding(10) };
            var card = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(43, 45, 49), Padding = new Padding(14), ColumnCount = 1, RowCount = 4 };
            card.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            card.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            card.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            card.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            card.Controls.Add(previewGame, 0, 0);
            card.Controls.Add(previewDetails, 0, 1);
            card.Controls.Add(previewState, 0, 2);
            card.Controls.Add(previewAssets, 0, 3);
            card.SizeChanged += (_, _) =>
            {
                int width = Math.Max(1, card.ClientSize.Width - 34);
                previewGame.MaximumSize = previewDetails.MaximumSize = previewState.MaximumSize = new Size(width, 0);
            };
            preview.Controls.Add(card);
            preview.Controls.Add(previewSource);
            right.Controls.Add(preview, 0, 0);

            placeholderList.Columns.Add("Placeholder", 150);
            placeholderList.Columns.Add("Description", 260);
            assetList.Columns.Add("Asset key", 150);
            assetList.Columns.Add("Description", 260);
            var placeholderPage = new TabPage("Placeholders");
            var insertPlaceholder = new Button { Text = "Insert selected placeholder", Dock = DockStyle.Bottom, Height = 32 };
            insertPlaceholder.Click += (_, _) => InsertPlaceholder();
            placeholderPage.Controls.Add(placeholderList);
            placeholderPage.Controls.Add(placeholderSearch);
            placeholderPage.Controls.Add(insertPlaceholder);
            var assetPage = new TabPage("Assets");
            var insertAsset = new Button { Text = "Use selected asset", Dock = DockStyle.Bottom, Height = 32 };
            insertAsset.Click += (_, _) => InsertAsset();
            assetPage.Controls.Add(assetList);
            assetPage.Controls.Add(assetSearch);
            assetPage.Controls.Add(insertAsset);
            referenceTabs.TabPages.AddRange(new[] { placeholderPage, assetPage });
            right.Controls.Add(referenceTabs, 0, 1);
            split.Panel2.Controls.Add(right);
            layout.Controls.Add(split, 0, 1);
            layout.Controls.Add(validation, 0, 2);
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            var close = new Button { Text = "Close", AutoSize = true };
            close.Click += (_, _) => Close();
            var folder = new Button { Text = "Open config folder", AutoSize = true };
            folder.Click += (_, _) => OpenFolder();
            actions.Controls.Add(close);
            actions.Controls.Add(saveButton);
            actions.Controls.Add(reloadButton);
            actions.Controls.Add(folder);
            layout.Controls.Add(actions, 0, 3);
            Controls.Add(layout);
            insertionTarget = fieldEditors["Details"];
        }

        private void LoadFiles(string? initialGame)
        {
            string directory = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "Config");
            try
            {
                var files = Directory.Exists(directory) ? Directory.EnumerateFiles(directory, "*.json", SearchOption.AllDirectories).Order(StringComparer.OrdinalIgnoreCase).ToArray() : Array.Empty<string>();
                binding = true;
                foreach (var path in files)
                {
                    fileSelector.Items.Add(new ConfigFile(path, System.IO.Path.GetRelativePath(directory, path)));
                }
                binding = false;
                int initial = 0;
                if (!string.IsNullOrEmpty(initialGame))
                {
                    for (int index = 0; index < files.Length; index++)
                    {
                        try
                        {
                            if (ConfigurationEditorDocument.Load(files[index]).GetGames().Contains(initialGame, StringComparer.OrdinalIgnoreCase))
                            {
                                initial = index;
                                break;
                            }
                        }
                        catch (Exception exception) when (IsFileError(exception) || exception is JsonException or ArgumentException or InvalidOperationException) { }
                    }
                }
                if (files.Length > 0)
                {
                    fileSelector.SelectedIndex = initial;
                    if (initialGame != null && gameSelector.Items.Contains(initialGame)) gameSelector.SelectedItem = initialGame;
                }
                else
                {
                    validation.Text = "No configuration files were found in Assets/Config.";
                    reloadButton.Enabled = false;
                }
            }
            catch (Exception exception) when (IsFileError(exception))
            {
                ShowError(exception.Message);
            }
            finally { binding = false; }
        }

        private void SelectFile()
        {
            if (binding || fileSelector.SelectedItem is not ConfigFile file)
            {
                return;
            }
            if (!ConfirmUnsaved())
            {
                binding = true;
                fileSelector.SelectedIndex = loadedFileIndex;
                binding = false;
                return;
            }
            try
            {
                document = ConfigurationEditorDocument.Load(file.Path);
                loadedFileIndex = fileSelector.SelectedIndex;
                referenceGame = "";
                SyncJson();
                RefreshView();
                if (!valid) editorTabs.SelectedIndex = 1;
            }
            catch (Exception exception) when (IsFileError(exception))
            {
                binding = true;
                fileSelector.SelectedIndex = loadedFileIndex;
                binding = false;
                ShowError(exception.Message);
            }
        }

        private void ReloadDocument()
        {
            if (document == null || !ConfirmUnsaved()) return;
            try
            {
                document = ConfigurationEditorDocument.Load(document.Path);
                referenceGame = "";
                SyncJson();
                RefreshView();
            }
            catch (Exception exception) when (IsFileError(exception)) { ShowError(exception.Message); }
        }

        private void FieldChanged(string field, string value)
        {
            if (binding || document == null || gameSelector.SelectedItem is not string game || stateSelector.SelectedItem is not string state) return;
            try
            {
                document.SetField(game, state, field, value);
                SyncJson();
                ScheduleRefresh();
            }
            catch (Exception exception) when (exception is JsonException or InvalidDataException or InvalidOperationException or ArgumentException) { ShowError(exception.Message); }
        }

        private void SyncJson()
        {
            binding = true;
            jsonEditor.Text = document?.Text ?? "";
            binding = false;
        }

        private void ScheduleRefresh()
        {
            valid = false;
            editTimer.Stop();
            editTimer.Start();
            saveButton.Enabled = false;
            UpdateTitle();
        }

        private void RefreshView()
        {
            if (document == null) return;
            valid = document.TryValidate(out var error);
            fieldsPage.Enabled = valid;
            saveButton.Enabled = valid && document.IsDirty;
            UpdateTitle();
            if (!valid)
            {
                validation.ForeColor = Color.Firebrick;
                validation.Text = error;
                previewSource.Text = "Preview unavailable until JSON is valid";
                previewGame.Text = previewDetails.Text = previewState.Text = previewAssets.Text = "";
                return;
            }
            binding = true;
            try
            {
                Rebind(gameSelector, document.GetGames());
                string game = gameSelector.SelectedItem as string ?? "";
                Rebind(stateSelector, game.Length == 0 ? Array.Empty<string>() : document.GetStates(game));
                string state = stateSelector.SelectedItem as string ?? "";
                foreach (var field in fieldEditors)
                {
                    field.Value.Enabled = state.Length != 0;
                    string value = state.Length == 0 ? "" : document.GetField(game, state, field.Key);
                    if (field.Value.Text != value) field.Value.Text = value;
                }
                if (referenceGame != game)
                {
                    referenceGame = game;
                    string configName = System.IO.Path.GetFileNameWithoutExtension(document.Path);
                    placeholders = ConfigurationEditorCatalog.GetPlaceholders(game, configName);
                    assets = ConfigurationEditorCatalog.GetAssets(game, configName);
                    FilterReferences();
                }
            }
            finally { binding = false; }
            if (stateSelector.SelectedItem == null)
            {
                previewGame.Text = previewDetails.Text = previewState.Text = previewAssets.Text = "";
                previewSource.Text = "No presence state selected";
                validation.ForeColor = SystemColors.GrayText;
                validation.Text = "This game has no presence states. You can add one in the JSON tab.";
                return;
            }
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (!valid || document == null || gameSelector.SelectedItem is not string game || stateSelector.SelectedItem is not string state)
            {
                return;
            }
            bool live = PresencePreviewStore.TryGet(game, state, out var values);
            if (!live)
            {
                var samples = placeholders.ToDictionary(item => item.Name, item => (object)item.Example, StringComparer.OrdinalIgnoreCase);
                if (samples.ContainsKey("state")) samples["state"] = state;
                if (samples.ContainsKey("gamestate")) samples["gamestate"] = state;
                values = samples;
            }
            var warnings = new List<string>();
            bool complete = ConfigurationEditorCatalog.IsComplete(game, System.IO.Path.GetFileNameWithoutExtension(document.Path));
            var known = placeholders.Select(item => item.Name).Concat(values.Keys).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var rendered = new Dictionary<string, string>();
            foreach (var field in ConfigurationEditorDocument.Fields)
            {
                string template = document.GetField(game, state, field);
                foreach (Match match in Regex.Matches(template, @"\{\s*(?<name>[^{}]+?)\s*\}", RegexOptions.CultureInvariant))
                {
                    string name = match.Groups["name"].Value.Trim();
                    if (!known.Contains(name)) warnings.Add((complete ? "Unknown placeholder: {" : "Placeholder not listed for this game: {") + name + "}");
                }
                string value = TemplateRenderer.Render(template, values);
                int maximum = field is "LargeAsset" or "SmallAsset" ? 256 : field is "Button1Text" or "Button2Text" ? 32 : 128;
                if (field is not "Button1URL" and not "Button2URL" && value.Length > maximum)
                {
                    warnings.Add(FieldLabel(field) + $" exceeds {maximum} characters and will be shortened.");
                    value = value[..(maximum - 1)] + "…";
                }
                rendered[field] = value;
            }
            previewSource.Text = live ? "Live values for the selected game and state" : "Sample values — the game does not need to be running";
            previewGame.Text = game;
            previewDetails.Text = rendered["Details"];
            previewState.Text = rendered["State"];
            var lines = new List<string>();
            AddPreviewLine(lines, "Large image key", rendered["LargeAsset"]);
            AddPreviewLine(lines, "Image tooltip", rendered["LargeAssetText"]);
            AddPreviewLine(lines, "Small image key", rendered["SmallAsset"]);
            AddPreviewLine(lines, "Small tooltip", rendered["SmallAssetText"]);
            for (int index = 1; index <= 2; index++)
            {
                string label = rendered[$"Button{index}Text"];
                string url = rendered[$"Button{index}URL"];
                if (string.IsNullOrWhiteSpace(label) && string.IsNullOrWhiteSpace(url)) continue;
                if (string.IsNullOrWhiteSpace(label) || !Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
                {
                    warnings.Add($"Button {index} needs a label and a valid HTTP(S) URL to appear.");
                }
                else
                {
                    lines.Add($"Button {index}: {label}");
                    lines.Add(url);
                }
            }
            previewAssets.Text = string.Join(Environment.NewLine, lines);
            validation.ForeColor = warnings.Count == 0 ? SystemColors.GrayText : Color.DarkOrange;
            validation.Text = warnings.Count == 0 ? (document.IsDirty ? "Unsaved changes. Save applies this file to the running presence." : "No unsaved changes. Assets are shown as keys; hosted images are not loaded in this preview.") : string.Join(Environment.NewLine, warnings.Distinct());
        }

        private void FilterReferences()
        {
            placeholderList.BeginUpdate();
            placeholderList.Items.Clear();
            foreach (var item in placeholders.Where(item => Matches(item.Name, item.Description, placeholderSearch.Text)))
            {
                placeholderList.Items.Add(new ListViewItem(new[] { "{" + item.Name + "}", item.Description }) { Tag = item.Name });
            }
            placeholderList.EndUpdate();
            assetList.BeginUpdate();
            assetList.Items.Clear();
            foreach (var item in assets.Where(item => Matches(item.Key, item.Description, assetSearch.Text)))
            {
                assetList.Items.Add(new ListViewItem(new[] { item.Key, item.Description }) { Tag = item.Key });
            }
            assetList.EndUpdate();
        }

        private void InsertPlaceholder()
        {
            if (placeholderList.SelectedItems.Count == 0 || insertionTarget == null || !insertionTarget.Enabled) return;
            string text = "{" + placeholderList.SelectedItems[0].Tag + "}";
            insertionTarget.SelectedText = text;
            insertionTarget.Focus();
        }

        private void InsertAsset()
        {
            if (assetList.SelectedItems.Count == 0) return;
            var target = insertionTarget == fieldEditors["SmallAsset"] ? fieldEditors["SmallAsset"] : fieldEditors["LargeAsset"];
            if (!target.Enabled) return;
            target.Text = assetList.SelectedItems[0].Tag as string ?? "";
            editorTabs.SelectedIndex = 0;
            target.Focus();
        }

        private bool SaveDocument()
        {
            if (document == null) return true;
            try
            {
                document.Save();
                RefreshView();
                return true;
            }
            catch (Exception exception) when (IsFileError(exception))
            {
                ShowError(exception.Message);
                return false;
            }
        }

        private bool ConfirmUnsaved()
        {
            if (document?.IsDirty != true) return true;
            var result = MessageBox.Show(this, "Save your changes before continuing?", "Unsaved configuration", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            return result == DialogResult.No || result == DialogResult.Yes && SaveDocument();
        }

        private void OpenFolder()
        {
            try
            {
                string path = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "Config");
                Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
            }
            catch (Exception exception) when (IsFileError(exception) || exception is System.ComponentModel.Win32Exception) { ShowError(exception.Message); }
        }

        private void UpdateTitle()
        {
            Text = "MultiPresence — Config editor" + (document?.IsDirty == true ? " *" : "");
        }

        private void ShowError(string message)
        {
            MessageBox.Show(this, message, "MultiPresence config editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static void Rebind(ComboBox selector, string[] items)
        {
            string? selected = selector.SelectedItem as string;
            if (selector.Items.Cast<string>().SequenceEqual(items)) return;
            selector.Items.Clear();
            selector.Items.AddRange(items);
            if (selected != null && selector.Items.Contains(selected)) selector.SelectedItem = selected;
            else if (items.Length > 0) selector.SelectedIndex = 0;
        }

        private static bool Matches(string name, string description, string filter) =>
            name.Contains(filter, StringComparison.OrdinalIgnoreCase) || description.Contains(filter, StringComparison.OrdinalIgnoreCase);

        private static bool IsFileError(Exception exception) => exception is IOException or UnauthorizedAccessException or InvalidDataException or System.Text.DecoderFallbackException or System.Text.EncoderFallbackException;

        private static void AddPreviewLine(List<string> lines, string label, string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) lines.Add(label + ": " + value);
        }

        private static string FieldLabel(string field) => field switch
        {
            "LargeAsset" => "Large image key",
            "SmallAsset" => "Small image key",
            "LargeAssetText" => "Large tooltip",
            "SmallAssetText" => "Small tooltip",
            "Button1Text" => "Button 1 label",
            "Button1URL" => "Button 1 URL",
            "Button2Text" => "Button 2 label",
            "Button2URL" => "Button 2 URL",
            _ => field
        };

        private sealed record ConfigFile(string Path, string Name)
        {
            public override string ToString() => Name;
        }
    }
}

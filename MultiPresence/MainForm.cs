using DiscordRPC;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using MultiPresence.Presence;
using MultiPresence.Properties;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Text.Json;
using System.Timers;

namespace MultiPresence
{
    public partial class MainForm : Form
    {
        public static System.Timers.Timer gameUpdater = new System.Timers.Timer(3000);
        public static bool status = false;
        public static bool isBlacklistLoaded = false;
        private string _lastDisabledGame = null;
        private bool _disabledNotificationSent = false;

        private static readonly string gameStatusUrl = "https://dekirai.crygod.de/multipresence/gamestatus.json";
        private static Dictionary<string, bool> gameEnabled = new();

        private static readonly string githubRepo = "Dekirai/MultiPresence";
        private static readonly string currentVersion = "24.10.2025";
        private static readonly string tempUpdaterPath = Path.Combine(Path.GetTempPath(), "Updater.exe");

        public MainForm()
        {
            InitializeComponent();

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            cb_DisableNotifications.Checked = Settings.Default.Notifications;
            cb_LaunchWithWindowsAdmin.Checked = Settings.Default.startupadmin;
            cb_DisableAutoUpdates.Checked = Settings.Default.autoupdate;

            gameUpdater.Elapsed += new ElapsedEventHandler(gameUpdater_Tick);
            gameUpdater.Interval = 5000;
            gameUpdater.Enabled = true;
            if (!cb_DisableAutoUpdates.Checked)
                CheckForUpdate();
            gameUpdater.Start();
            lb_Version.Text = $"Version {currentVersion}";
        }

        public void CheckForUpdate()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("User-Agent", "CSharpApp");
                    string apiUrl = $"https://api.github.com/repos/{githubRepo}/releases/latest";
                    string jsonResponse = client.DownloadString(apiUrl);

                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        JsonElement root = doc.RootElement;
                        string latestVersion = root.GetProperty("tag_name").GetString()?.Trim('v');
                        string releaseUrl = root.GetProperty("html_url").GetString();

                        if (IsNewerVersion(latestVersion, currentVersion))
                        {
                            DialogResult result = MessageBox.Show(
                                $"A new version ({latestVersion}) is available. Do you want to update?",
                                "MultiPresence - Update available",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.ServiceNotification
                            );

                            if (result == DialogResult.Yes)
                            {
                                string updateUrl = null;
                                string updaterUrl = null;
                                string actualZipName = null;

                                JsonElement assets = root.GetProperty("assets");

                                foreach (JsonElement asset in assets.EnumerateArray())
                                {
                                    string name = asset.GetProperty("name").GetString();

                                    if (name.Equals("Updater.exe", StringComparison.OrdinalIgnoreCase))
                                    {
                                        updaterUrl = asset.GetProperty("browser_download_url").GetString();
                                    }
                                    else if (name.Equals("MultiPresence.zip", StringComparison.OrdinalIgnoreCase))
                                    {
                                        updateUrl = asset.GetProperty("browser_download_url").GetString();
                                        actualZipName = name;
                                    }
                                }

                                if (updaterUrl != null && updateUrl != null)
                                {
                                    string dynamicZipPath = Path.Combine(Path.GetTempPath(), actualZipName);

                                    DownloadFile(updaterUrl, tempUpdaterPath);
                                    DownloadFile(updateUrl, dynamicZipPath);

                                    KillProcess("MultiPresenceGame");

                                    DialogResult result2 = MessageBox.Show(
                                        $"Do you want to view the changelogs?",
                                        "MultiPresence - Update available",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Information,
                                        MessageBoxDefaultButton.Button1,
                                        MessageBoxOptions.ServiceNotification
                                    );

                                    if (result2 == DialogResult.Yes)
                                    {
                                        Process.Start(new ProcessStartInfo
                                        {
                                            FileName = releaseUrl,
                                            UseShellExecute = true
                                        });
                                    }

                                    Process.Start(tempUpdaterPath, $"\"{dynamicZipPath}\" \"{Application.ExecutablePath}\" \"{tempUpdaterPath}\"");
                                    BalloonUpdate("Update downloaded, MultiPresence is now restarting and updating files!");
                                    Environment.Exit(0);
                                }
                                else
                                {
                                    MessageBox.Show("Update or Updater file not found in the release assets.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BalloonUpdate("Failed to update MultiPresence!");
                MessageBox.Show($"Error checking for updates: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private static void DownloadFile(string url, string destination)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, destination);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading {Path.GetFileName(destination)}: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool IsNewerVersion(string latest, string current)
        {
            try
            {
                DateTime latestDate = DateTime.ParseExact(latest, "dd.MM.yyyy", null);
                DateTime currentDate = DateTime.ParseExact(current, "dd.MM.yyyy", null);
                return latestDate > currentDate;
            }
            catch
            {
                return false;
            }
        }

        private static void KillProcess(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                }
                catch { }
            }
        }

        public class GameConfig
        {
            [JsonProperty("enabled")]
            public bool Enabled { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }
        }

        private async void gameUpdater_Tick(object sender, EventArgs e)
        {
            string game = GameDetector.GetGame();
            string json;
            Dictionary<string, GameConfig> gameConfigs;

            if (File.Exists("Assets\\steam_appid.txt"))
                File.Delete("Assets\\steam_appid.txt");

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("CSharpApp");
                var gamestatus = await client.GetStringAsync(gameStatusUrl);
                gameConfigs = JsonConvert
                    .DeserializeObject<Dictionary<string, GameConfig>>(gamestatus)
                    ?? new Dictionary<string, GameConfig>();
            }
            catch
            {
                gameConfigs = new Dictionary<string, GameConfig>();
            }

            bool enabled = true;
            if (gameConfigs.TryGetValue(game, out var cfg))
                enabled = cfg.Enabled;

            if (!enabled)
            {
                if (!_disabledNotificationSent || _lastDisabledGame != game)
                {
                    //BalloonDisabled(game);
                    _disabledNotificationSent = true;
                    _lastDisabledGame = game;
                }
                return;
            }

            _disabledNotificationSent = false;
            _lastDisabledGame = null;

            if (gameEnabled.TryGetValue(game, out bool isEnabled) && !isEnabled)
                return;

            if (!status)
            {
                PlaceholderHelper._startTimestamp = Timestamps.Now;
                switch (game)
                {
                    case "Call of Duty®":
                        Balloon(game);
                        await COD.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "CODE VEIN":
                        Balloon(game);
                        CV.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Crash Bandicoot 4: It's About Time":
                        Balloon(game);
                        CB4.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Crash Bandicoot N. Sane Trilogy":
                        Balloon(game);
                        CBNT.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "CRISIS CORE –FINAL FANTASY VII– REUNION":
                        Balloon(game);
                        CCFFVII.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Dark Souls II":
                        Balloon(game);
                        DSII.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Dark Souls III":
                        Balloon(game);
                        DSIII.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Dark Souls: Remastered":
                        Balloon(game);
                        DSR.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Death Stranding":
                        Balloon(game);
                        DSDC.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Devil May Cry":
                        Balloon(game);
                        DMC1.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Devil May Cry 2":
                        Balloon(game);
                        DMC2.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Devil May Cry 3":
                        Balloon(game);
                        DMC3.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Devil May Cry 4":
                        Balloon(game);
                        DMC4.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Devil May Cry 5":
                        Balloon(game);
                        DMC5.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Digimon Story Time Stranger":
                        Balloon(game);
                        DSTS.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "DmC Devil May Cry":
                        Balloon(game);
                        DMC.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Elden Ring":
                        Balloon(game);
                        ER.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Final Fantasy VII Rebirth":
                        Balloon(game);
                        FFVIIRB.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Final Fantasy VII Remake":
                        Balloon(game);
                        FFVIIR.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Final Fantasy XV":
                        Balloon(game);
                        FFXV.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Final Fantasy XVI":
                        Balloon(game);
                        FFXVI.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Granblue Fantasy: Relink":
                        Balloon(game);
                        GBFR.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Gunfire Reborn":
                        Balloon(game);
                        await GFR.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Hello Kitty Island Adventure":
                        Balloon(game);
                        await HK.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Hogwarts Legacy":
                        Balloon(game);
                        await HL.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Kingdom Hearts Birth by Sleep Final Mix":
                        Balloon(game);
                        KHBBS.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Kingdom Hearts Dream Drop Distance":
                        Balloon(game);
                        KHDDD.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Kingdom Hearts Final Mix":
                        Balloon(game);
                        KH1.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Kingdom Hearts II Final Mix":
                        Balloon(game);
                        KH2.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Kingdom Hearts III":
                        Balloon(game);
                        KH3.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Kingdom Hearts Re:Chain of Memories":
                        Balloon(game);
                        KHCOM.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Labyrinthine":
                        Balloon(game);
                        LR.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Lies of P":
                        Balloon(game);
                        LOP.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Mega Man 11":
                        Balloon(game);
                        MM11.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Mega Man Battle Network 6: Cybeast Falzar":
                        Balloon(game);
                        MMBN6F.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Mega Man Battle Network 6: Cybeast Gregar":
                        Balloon(game);
                        MMBN6G.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Marvel's Spider-Man 2":
                        Balloon(game);
                        MSM2.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Marvel's Spider-Man Remastered":
                        Balloon(game);
                        MSMR.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Marvel's Spider-Man: Miles Morales":
                        Balloon(game);
                        MSMMMM.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Overwatch 2":
                        Balloon(game);
                        await OW.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Pangya Reborn":
                        Balloon(game);
                        PYRE.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Persona 4 Golden":
                        Balloon(game);
                        P4G.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Persona 5 Strikers":
                        Balloon(game);
                        P5S.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Persona 5 Royal":
                        Balloon(game);
                        P5R.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Persona 5: The Phantom X":
                        Balloon(game);
                        P5X.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Project Diva Mega Mix+":
                        Balloon(game);
                        PDMM.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Resident Evil":
                        Balloon(game);
                        RE.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Resident Evil 2":
                        Balloon(game);
                        RE2R.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Resident Evil 4 (2005)":
                        Balloon(game);
                        RE4.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Resident Evil 4 Remake":
                        Balloon(game);
                        RE4R.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Resident Evil 5":
                        Balloon(game);
                        RE5.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Resident Evil 6":
                        Balloon(game);
                        RE6.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Resident Evil 7":
                        Balloon(game);
                        RE7.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Resident Evil 8":
                        Balloon(game);
                        RE8.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Resident Evil Revelations 2":
                        Balloon(game);
                        REV2.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Scott Pilgrim vs The World":
                        Balloon(game);
                        SPTG.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Sonic Adventure 2":
                        Balloon(game);
                        SA2.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Sonic Adventure DX":
                        Balloon(game);
                        SADX.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Sonic Generations":
                        Balloon(game);
                        SXSG.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Stellar Blade":
                        Balloon(game);
                        SB.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Team Fortress 2":
                        Balloon(game);
                        await TF2.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Temtem: Swarm":
                        Balloon(game);
                        TTS.DoAction();
                        gameUpdater.Stop();
                        break;
                    //case "The Binding of Isaac: Rebirth":
                    //    Balloon(game);
                    //    TBOI.DoAction();
                    //    gameUpdater.Stop();
                    //    break;
                    case "The Witcher 3":
                        Balloon(game);
                        TWIII.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "TY the Tasmanian Tiger":
                        Balloon(game);
                        TY.DoAction();
                        gameUpdater.Stop();
                        break;
                    //case "Vampire Survivors":
                    //    Balloon(game);
                    //    VS.DoAction();
                    //    gameUpdater.Stop();
                    //    break;
                    case "Visions of Mana":
                        Balloon(game);
                        VOM.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Zelda: The Wind Waker HD":
                        Balloon(game);
                        WWHD.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Zelda: Twilight Princess HD":
                        Balloon(game);
                        TPHD.DoAction();
                        gameUpdater.Stop();
                        break;
                }
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            try
            {
                var processes = Process.GetProcessesByName("MultiPresenceGame");
                if (processes.Any())
                {
                    foreach (var process in processes)
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }

                Settings.Default.Notifications = cb_DisableNotifications.Checked;
                Settings.Default.autoupdate = cb_DisableAutoUpdates.Checked;
                Settings.Default.Save();
                Application.Exit();
            }
            catch
            {
                Settings.Default.Notifications = cb_DisableNotifications.Checked;
                Settings.Default.autoupdate = cb_DisableAutoUpdates.Checked;
                Settings.Default.Save();
                Application.Exit();
            }
        }

        private void Balloon(string text)
        {
            if (cb_DisableNotifications.Checked)
                return;
            notify.BalloonTipTitle = "System";
            notify.BalloonTipText = $"Keeping track of {text}.";
            notify.ShowBalloonTip(3000);
        }

        private void BalloonUpdate(string text)
        {
            notify.BalloonTipTitle = "MultiPresence - Update status";
            notify.BalloonTipText = text;
            notify.ShowBalloonTip(3000);
        }

        private void BalloonDisabled(string game)
        {
            if (cb_DisableNotifications.Checked) return;
            notify.BalloonTipTitle = "Game Disabled";
            notify.BalloonTipText = $"{game} is currently disabled because it requires an update.";
            notify.ShowBalloonTip(3000);
        }

        private async void btn_Config_Click(object sender, EventArgs e)
        {
            string configFolderPath = Path.Combine(Environment.CurrentDirectory, "Assets/Config");

            if (Directory.Exists(configFolderPath) &&
                Directory.GetFiles(configFolderPath).Length > 0)
            {
                Process.Start(new ProcessStartInfo(configFolderPath) { UseShellExecute = true });
            }
            else
            {
                DialogResult result = MessageBox.Show("The config folder is empty, do you want to download the config files?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    Directory.CreateDirectory(configFolderPath);

                    string url = "https://raw.githubusercontent.com/Dekirai/MultiPresence/main/MultiPresence/Config.zip";
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            HttpResponseMessage response = await client.GetAsync(url);
                            response.EnsureSuccessStatusCode();
                            byte[] content = await response.Content.ReadAsByteArrayAsync();

                            string zipFilePath = Path.Combine(configFolderPath, "Config.zip");
                            File.WriteAllBytes(zipFilePath, content);

                            ZipFile.ExtractToDirectory(zipFilePath, configFolderPath, overwriteFiles: true);

                            File.Delete(zipFilePath);

                            MessageBox.Show($"The config folder has been set up at {configFolderPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Process.Start(new ProcessStartInfo(configFolderPath) { UseShellExecute = true });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

            }
        }

        private async void btn_Blacklist_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\Assets\\blacklist.json"))
            {
                DialogResult result = MessageBox.Show("The blacklist file does not exist, do you want me to create one?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    string url = "https://raw.githubusercontent.com/Dekirai/MultiPresence/main/MultiPresence/blacklist.json";
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            HttpResponseMessage response = await client.GetAsync(url);
                            response.EnsureSuccessStatusCode();
                            string content = await response.Content.ReadAsStringAsync();

                            await File.WriteAllTextAsync(Environment.CurrentDirectory + "\\Assets\\blacklist.json", content);

                            MessageBox.Show($"The file has been created and saved in {Environment.CurrentDirectory + "\\Assets\\blacklist.json"}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Process.Start(new ProcessStartInfo(Environment.CurrentDirectory + "\\Assets\\blacklist.json") { UseShellExecute = true });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
                Process.Start(new ProcessStartInfo(Environment.CurrentDirectory + "\\Assets\\blacklist.json") { UseShellExecute = true });
        }

        private void SetStartupAdmin(bool enable)
        {
            const string taskName = "MultiPresence";

            using (TaskService ts = new TaskService())
            {
                if (enable)
                {
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = "Starts the application with elevated privileges at startup";
                    td.Principal.LogonType = TaskLogonType.InteractiveToken;
                    td.Principal.RunLevel = TaskRunLevel.Highest;

                    td.Triggers.Add(new LogonTrigger());

                    string exePath = Application.ExecutablePath;
                    td.Actions.Add(new ExecAction(exePath, null, null));

                    ts.RootFolder.RegisterTaskDefinition(taskName, td);
                }
                else
                {
                    // Remove the task
                    ts.RootFolder.DeleteTask(taskName, false);
                }
            }
        }

        private void SetStartup(bool enable)
        {
            const string appName = "MultiPresence";
            string exePath = Application.ExecutablePath;

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (enable)
            {
                // Add the application to startup
                registryKey.SetValue(appName, exePath);
            }
            else
            {
                // Remove the application from startup
                registryKey.DeleteValue(appName, false);
            }
        }

        private void cb_LaunchWithWindowsAdmin_Click(object sender, EventArgs e)
        {
            if (cb_LaunchWithWindowsAdmin.Checked)
            {
                cb_LaunchWithWindows.Checked = false;
                SetStartupAdmin(true);
                SetStartup(false);
                Settings.Default.startup = false;
                Settings.Default.startupadmin = true;
                Settings.Default.Save();
            }
            else
            {
                SetStartupAdmin(false);
                Settings.Default.startupadmin = false;
                Settings.Default.Save();
            }
        }

        private void cb_LaunchWithWindows_Click(object sender, EventArgs e)
        {
            if (cb_LaunchWithWindows.Checked)
            {
                cb_LaunchWithWindowsAdmin.Checked = false;
                SetStartup(true);
                SetStartupAdmin(false);
                Settings.Default.startup = true;
                Settings.Default.startupadmin = false;
                Settings.Default.Save();
            }
            else
            {
                SetStartup(false);
                Settings.Default.startup = false;
                Settings.Default.Save();
            }
        }

        private void cb_DisableNotifications_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Notifications = cb_DisableNotifications.Checked;
            Settings.Default.Save();
        }

        private void cb_DisableAutoUpdates_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.autoupdate = cb_DisableAutoUpdates.Checked;
            Settings.Default.Save();
        }
    }
}
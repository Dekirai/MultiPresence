using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using MultiPresence.Presence;
using MultiPresence.Properties;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Timers;

namespace MultiPresence
{
    public partial class MainForm : Form
    {
        public static System.Timers.Timer gameUpdater = new System.Timers.Timer(3000);
        Blacklist? blacklist = null;
        public static bool status = false;
        public static bool isBlacklistLoaded = false;

        private static readonly string githubRepo = "Dekirai/MultiPresence";
        private static readonly string currentVersion = "04.03.2025";
        private static readonly string tempUpdaterPath = Path.Combine(Path.GetTempPath(), "Updater.exe");
        private static readonly string tempUpdateZip = Path.Combine(Path.GetTempPath(), "update.zip");

        public MainForm()
        {
            InitializeComponent();
            CheckForUpdate();

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            cb_DisableNotifications.Checked = Settings.Default.Notifications;
            cb_LaunchWithWindowsAdmin.Checked = Settings.Default.startupadmin;

            gameUpdater.Elapsed += new ElapsedEventHandler(gameUpdater_Tick);
            gameUpdater.Interval = 5000;
            gameUpdater.Enabled = true;

            gameUpdater.Start();
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

                                string updateUrl = root.GetProperty("assets")[0].GetProperty("browser_download_url").GetString();
                                string updaterUrl = root.GetProperty("assets")[1].GetProperty("browser_download_url").GetString();

                                DownloadFile(updaterUrl, tempUpdaterPath);
                                DownloadFile(updateUrl, tempUpdateZip);

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

                                Process.Start(tempUpdaterPath, $"\"{tempUpdateZip}\" \"{Application.ExecutablePath}\" \"{tempUpdaterPath}\"");
                                BalloonUpdate("Update completed, MultiPresence is now restarting!");
                                Environment.Exit(0);
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

        private async void gameUpdater_Tick(object sender, EventArgs e)
        {
            string game = await GameDetector.GetGameAsync();
            string json;

            if (File.Exists("blacklist.json"))
            {
                json = File.ReadAllText("blacklist.json");
                blacklist = JsonConvert.DeserializeObject<Blacklist>(json);
                isBlacklistLoaded = true;
            }
            if (File.Exists("steam_appid.txt"))
                File.Delete("steam_appid.txt");

            if (isBlacklistLoaded && blacklist != null)
                status = blacklist.GetValue(game);

            if (!status)
            {
                switch (game)
                {
                    case "Call of Duty®":
                        Balloon(game);
                        await COD.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "CRISIS CORE –FINAL FANTASY VII– REUNION":
                        Balloon(game);
                        CCFFVII.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Dark Souls: Remastered":
                        Balloon(game);
                        DSR.DoAction();
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
                    case "Elden Ring":
                        Balloon(game);
                        ER.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Gunfire Reborn":
                        Balloon(game);
                        await GFR.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Final Fantasy VII Remake":
                        Balloon(game);
                        FFVIIR.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Final Fantasy VII Rebirth":
                        Balloon(game);
                        FFVIIRB.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Final Fantasy XVI":
                        Balloon(game);
                        FFXVI.DoAction();
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
                    case "Kingdom Hearts Re:Chain of Memories":
                        Balloon(game);
                        KHCOM.DoAction();
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
                    case "Sonic Adventure DX":
                        Balloon(game);
                        SADX.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Sonic Adventure 2":
                        Balloon(game);
                        SA2.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Overwatch 2":
                        Balloon(game);
                        await OW.DoAction();
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
                    case "TY the Tasmanian Tiger":
                        Balloon(game);
                        TY.DoAction();
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
                    case "Resident Evil":
                        Balloon(game);
                        RE.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Resident Evil Revelations 2":
                        Balloon(game);
                        REV2.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Pangya Reborn":
                        Balloon(game);
                        PYRE.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Marvel's Spider-Man Remastered":
                        Balloon(game);
                        MSMR.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Marvel's Spider-Man 2":
                        Balloon(game);
                        MSM2.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Marvel's Spider-Man: Miles Morales":
                        Balloon(game);
                        MSMMMM.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Mega Man Battle Network 6: Cybeast Gregar":
                        Balloon(game);
                        MMBN6G.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Mega Man Battle Network 6: Cybeast Falzar":
                        Balloon(game);
                        MMBN6F.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Temtem: Swarm":
                        Balloon(game);
                        TTS.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Vampire Survivors":
                        Balloon(game);
                        VS.DoAction();
                        gameUpdater.Stop();
                        break;
                    case "Visions of Mana":
                        Balloon(game);
                        VOM.DoAction();
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
                Settings.Default.Save();
                Application.Exit();
            }
            catch
            {
                Settings.Default.Notifications = cb_DisableNotifications.Checked;
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

        private async void btn_Config_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\config.json"))
            {
                DialogResult result = MessageBox.Show("The config file does not exist, do you want me to create one?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    string url = "https://raw.githubusercontent.com/Dekirai/MultiPresence/main/MultiPresence/config.json";
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            HttpResponseMessage response = await client.GetAsync(url);
                            response.EnsureSuccessStatusCode();
                            string content = await response.Content.ReadAsStringAsync();

                            await File.WriteAllTextAsync(Environment.CurrentDirectory + "\\config.json", content);

                            MessageBox.Show($"The file has been created and saved in {Environment.CurrentDirectory + "\\config.json"}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Process.Start(new ProcessStartInfo(Environment.CurrentDirectory + "\\config.json") { UseShellExecute = true });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
                Process.Start(new ProcessStartInfo(Environment.CurrentDirectory + "\\config.json") { UseShellExecute = true });
        }

        private async void btn_Blacklist_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\blacklist.json"))
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

                            await File.WriteAllTextAsync(Environment.CurrentDirectory + "\\blacklist.json", content);

                            MessageBox.Show($"The file has been created and saved in {Environment.CurrentDirectory + "\\blacklist.json"}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Process.Start(new ProcessStartInfo(Environment.CurrentDirectory + "\\blacklist.json") { UseShellExecute = true });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
                Process.Start(new ProcessStartInfo(Environment.CurrentDirectory + "\\blacklist.json") { UseShellExecute = true });
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
    }
}
using MultiPresence.Presence;
using System.Diagnostics;
using System.Timers;
using MultiPresence.Properties;
using Newtonsoft.Json;

namespace MultiPresence
{
    public partial class MainForm : Form
    {
        public static System.Timers.Timer gameUpdater = new System.Timers.Timer(3000);
        Blacklist blacklist = null;
        public static bool status = false;
        public static bool isBlacklistLoaded = false;
        public MainForm()
        {
            InitializeComponent();

            cb_DisableNotifications.Checked = Settings.Default.Notifications;
            cb_english.Checked = Settings.Default.langEN;
            cb_german.Checked = Settings.Default.langDE;

            gameUpdater.Elapsed += new ElapsedEventHandler(gameUpdater_Tick);
            gameUpdater.Interval = 5000;
            gameUpdater.Enabled = true;

            gameUpdater.Start();
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

            if (isBlacklistLoaded && blacklist != null)
                status = blacklist.GetValue(game);

            if (!status)
            {
                switch (game)
                {
                    case "AsobiSW":
                        Balloon(game);
                        ASW.DoAction();
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
                    case "Sonic Adventure 2":
                        Balloon(game);
                        SA2.DoAction();
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
                }
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Settings.Default.Notifications = cb_DisableNotifications.Checked;
            Settings.Default.Save();
            Application.Exit();
        }

        private void Balloon(string text)
        {
            if (cb_DisableNotifications.Checked)
                return;
            notify.BalloonTipTitle = "System";
            notify.BalloonTipText = $"Keeping track of {text}.";
            notify.ShowBalloonTip(3000);
        }

        private void cb_english_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_english.Checked == true)
            {
                cb_german.Checked = false;
                Settings.Default.langEN = cb_english.Checked = true;
                Settings.Default.langDE = cb_german.Checked = false;
                Settings.Default.Save();
            }
        }

        private void cb_german_Click(object sender, EventArgs e)
        {
            if (cb_german.Checked == true)
            {
                cb_english.Checked = false;
                Settings.Default.langEN = cb_english.Checked = false;
                Settings.Default.langDE = cb_german.Checked = true;
                Settings.Default.Save();
            }
        }
    }
}
using MultiPresence.Presence;
using System.Diagnostics;
using System.Timers;
using MultiPresence.Properties;

namespace MultiPresence
{
    public partial class MainForm : Form
    {
        KH1 kh1 = new KH1();
        KH2 kh2 = new KH2();
        KH3 kh3 = new KH3();
        KHBBS khbbs = new KHBBS();
        KHDDD khddd = new KHDDD();
        RE4 re4 = new RE4();
        SA2 sa2 = new SA2();
        TPHD tphd = new TPHD();
        TY ty = new TY();
        MM11 mm11 = new MM11();
        WWHD wwhd = new WWHD();
        public static System.Timers.Timer gameUpdater = new System.Timers.Timer(3000);
        public MainForm()
        {
            InitializeComponent();

            cb_DisableNotifications.Checked = Settings.Default.Notifications;
            cb_DisableInfoNotifications.Checked = Settings.Default.InfoNotifcations;
            
            gameUpdater.Elapsed += new ElapsedEventHandler(gameUpdater_Tick);
            gameUpdater.Interval = 5000;
            gameUpdater.Enabled = true;

            gameUpdater.Start();
        }
        private void gameUpdater_Tick(object sender, EventArgs e)
        {
            var game_kh1 = Process.GetProcessesByName("KINGDOM HEARTS FINAL MIX");
            var game_kh2 = Process.GetProcessesByName("KINGDOM HEARTS II FINAL MIX");
            var game_kh3 = Process.GetProcessesByName("KINGDOM HEARTS III");
            var game_khbbs = Process.GetProcessesByName("KINGDOM HEARTS Birth by Sleep FINAL MIX");
            var game_khddd = Process.GetProcessesByName("KINGDOM HEARTS Dream Drop Distance");
            var game_cemu = Process.GetProcessesByName("Cemu");
            var game_mm11 = Process.GetProcessesByName("game");
            var game_re4 = Process.GetProcessesByName("bio4");
            var game_sa2 = Process.GetProcessesByName("sonic2app");
            var game_ty = Process.GetProcessesByName("TY");

            if (game_kh1.Length > 0)
            {
                Balloon("Kingdom Hearts Final Mix");
                kh1.DoAction();
                gameUpdater.Stop();
            }
            else if (game_kh2.Length > 0)
            {
                Balloon("Kingdom Hearts II Final Mix");
                kh2.DoAction();
                gameUpdater.Stop();
            }
            else if (game_kh3.Length > 0)
            {
                Balloon("Kingdom Hearts III");
                kh3.DoAction();
                gameUpdater.Stop();
            }
            else if (game_khbbs.Length > 0)
            {
                Balloon("Kingdom Hearts Birth by Sleep Final Mix");
                khbbs.DoAction();
                gameUpdater.Stop();
            }
            else if (game_khddd.Length > 0)
            {
                Balloon("Kingdom Hearts Dream Drop Distance");
                khddd.DoAction();
                gameUpdater.Stop();
            }
            else if (game_mm11.Length > 0)
            {
                var title = Process.GetProcessesByName("game").FirstOrDefault();

                if (title.MainWindowTitle.Contains("MEGAMAN11")) //Doing this because there might be other games with "game.exe"
                {
                    Balloon("Mega Man 11");
                    mm11.DoAction();
                    gameUpdater.Stop();
                    return;
                }

            }
            else if (game_re4.Length > 0)
            {
                Balloon("Resident Evil 4");
                BalloonInfo("I am currently scanning the memory. Your machine may lag for short period of time.");
                re4.DoAction();
                gameUpdater.Stop();
            }
            else if (game_sa2.Length > 0)
            {
                Balloon("Sonic Adventure 2");
                sa2.DoAction();
                gameUpdater.Stop();
            }
            else if (game_cemu.Length > 0)
            {
                var title = Process.GetProcessesByName("Cemu").FirstOrDefault();

                if (title.MainWindowTitle.Contains("Wind Waker HD"))
                {
                    Balloon("Zelda: The Wind Waker HD");
                    BalloonInfo("I am currently scanning the memory. Your machine may lag for short period of time.");
                    wwhd.DoAction();
                    gameUpdater.Stop();
                    return;
                }
                else if (title.MainWindowTitle.Contains("Twilight Princess HD"))
                {
                    Balloon("Zelda: Twilight Princess HD");
                    BalloonInfo("I am currently scanning the memory. Your machine may lag for short period of time.");
                    tphd.DoAction();
                    gameUpdater.Stop();
                    return;
                }
            }
            else if (game_ty.Length > 0)
            {
                Balloon("TY the Tasmanian Tiger");
                ty.DoAction();
                gameUpdater.Stop();
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Settings.Default.Notifications = cb_DisableNotifications.Checked;
            Settings.Default.InfoNotifcations = cb_DisableInfoNotifications.Checked;
            Settings.Default.Save();
            Application.Exit();
        }

        private void Balloon(string text)
        {
            if (cb_DisableNotifications.Checked)
                return;
            notify.BalloonTipTitle = "System";
            notify.BalloonTipText = $"I now keep track of {text}.";
            notify.ShowBalloonTip(3000);
        }

        private void BalloonInfo(string text)
        {
            if (cb_DisableInfoNotifications.Checked)
                return;
            notify.BalloonTipTitle = "Information";
            notify.BalloonTipText = $"{text}";
            notify.ShowBalloonTip(5000);
        }
    }
}
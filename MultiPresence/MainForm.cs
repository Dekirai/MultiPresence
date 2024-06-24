using MultiPresence.Presence;
using System.Diagnostics;
using System.Timers;
using MultiPresence.Properties;

namespace MultiPresence
{
    public partial class MainForm : Form
    {
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

        private async void gameUpdater_Tick(object sender, EventArgs e)
        {
            int game = await GameDetector.GetGameAsync();

            switch (game)
            {
                case 1:
                    Balloon("Kingdom Hearts Final Mix");
                    KH1.DoAction();
                    gameUpdater.Stop();
                    break;
                case 2:
                    Balloon("Kingdom Hearts II Final Mix");
                    KH2.DoAction();
                    gameUpdater.Stop();
                    break;
                //case 3:
                //    Balloon("Kingdom Hearts III");
                //    KH3.DoAction();
                //    gameUpdater.Stop();
                //    break;
                case 4:
                    Balloon("Kingdom Hearts Birth by Sleep Final Mix");
                    KHBBS.DoAction();
                    gameUpdater.Stop();
                    break;
                case 5:
                    Balloon("Kingdom Hearts Dream Drop Distance");
                    KHDDD.DoAction();
                    gameUpdater.Stop();
                    break;
                case 6:
                    Balloon("Mega Man 11");
                    MM11.DoAction();
                    gameUpdater.Stop();
                    break;
                case 7:
                    Balloon("Resident Evil 4");
                    BalloonInfo("I am currently scanning the memory. Your machine may lag for short period of time.");
                    RE4.DoAction();
                    gameUpdater.Stop();
                    break;
                case 8:
                    Balloon("Sonic Adventure 2");
                    SA2.DoAction();
                    gameUpdater.Stop();
                    break;
                case 9:
                    Balloon("Zelda: The Wind Waker HD");
                    BalloonInfo("I am currently scanning the memory. Your machine may lag for short period of time.");
                    WWHD.DoAction();
                    gameUpdater.Stop();
                    break;
                case 10:
                    Balloon("Zelda: Twilight Princess HD");
                    BalloonInfo("I am currently scanning the memory. Your machine may lag for short period of time.");
                    TPHD.DoAction();
                    gameUpdater.Stop();
                    break;
                case 11:
                    Balloon("Ratchet - Deadlocked");
                    RDL.DoAction();
                    gameUpdater.Stop();
                    break;
                case 12:
                    Balloon("Mega Man X - Command Mission");
                    MMXCM.DoAction();
                    gameUpdater.Stop();
                    break;
                case 13:
                    Balloon("TY the Tasmanian Tiger");
                    TY.DoAction();
                    gameUpdater.Stop();
                    break;
                case 14:
                    Balloon("Resident Evil 5");
                    RE5.DoAction();
                    gameUpdater.Stop();
                    break;
                case 15:
                    Balloon("Resident Evil 6");
                    RE6.DoAction();
                    gameUpdater.Stop();
                    break;
                case 16:
                    Balloon("Resident Evil");
                    RE.DoAction();
                    gameUpdater.Stop();
                    break;
                case 17:
                    Balloon("Resident Evil Revelations 2");
                    REV2.DoAction();
                    gameUpdater.Stop();
                    break;
                case 18:
                    Balloon("PangYa Reborn");
                    PYRE.DoAction();
                    gameUpdater.Stop();
                    break;
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
            notify.BalloonTipText = $"Keeping track of {text}.";
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
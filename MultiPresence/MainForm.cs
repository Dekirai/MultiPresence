using MultiPresence.Presence;
using System.Diagnostics;
using System.Timers;
using MultiPresence.Properties;
using Microsoft.Win32;

namespace MultiPresence
{
    public partial class MainForm : Form
    {

        KH2 kh2 = new KH2();
        WWHD wwhd = new WWHD();
        TY ty = new TY();
        public static System.Timers.Timer gameUpdater = new System.Timers.Timer(3000);
        public MainForm()
        {
            InitializeComponent();
            gameUpdater.Elapsed += new ElapsedEventHandler(gameUpdater_Tick);
            gameUpdater.Interval = 5000;
            gameUpdater.Enabled = true;

            notify.BalloonTipTitle = "MultiPresence";
            notify.BalloonTipText = "I am active now!";
            notify.ShowBalloonTip(3000);
            gameUpdater.Start();
        }
        private void gameUpdater_Tick(object sender, EventArgs e)
        {
            var game_kh2 = Process.GetProcessesByName("KINGDOM HEARTS II FINAL MIX");
            var game_cemu = Process.GetProcessesByName("Cemu");
            var game_ty = Process.GetProcessesByName("TY");

            if (game_kh2.Length > 0)
            {
                notify.BalloonTipTitle = "Found a game!";
                notify.BalloonTipText = "I now keep track of Kingdom Hearts II.";
                notify.ShowBalloonTip(3000);
                kh2.DoAction();
                gameUpdater.Stop();
            }
            else if (game_cemu.Length > 0)
            {
                var title = Process.GetProcessesByName("Cemu").FirstOrDefault();

                if (title.MainWindowTitle.Contains("Wind Waker HD"))
                {
                    notify.BalloonTipTitle = "Found a game!";
                    notify.BalloonTipText = "I now keep track of Wind Waker HD.";
                    notify.ShowBalloonTip(3000);
                    wwhd.DoAction();
                    gameUpdater.Stop();
                    return;
                }
            }
            else if (game_ty.Length > 0)
            {
                notify.BalloonTipTitle = "Found a game!";
                notify.BalloonTipText = "I now keep track of TY the Tasmanian Tiger.";
                notify.ShowBalloonTip(3000);
                ty.DoAction();
                gameUpdater.Stop();
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
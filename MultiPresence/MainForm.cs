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
            Process[] game_kh2 = Process.GetProcessesByName("KINGDOM HEARTS II FINAL MIX");
            Process[] game_cemu = Process.GetProcessesByName("Cemu");

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
                notify.BalloonTipTitle = "Found a game!";
                notify.BalloonTipText = "I now keep track of Cemu.";
                notify.ShowBalloonTip(3000);
                gameUpdater.Stop();
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
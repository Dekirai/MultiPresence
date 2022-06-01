using MultiPresence.Presence;
using System.Diagnostics;
using System.Timers;

namespace MultiPresence
{
    public partial class MainForm : Form
    {
        KH1 kh1 = new KH1();
        KH2 kh2 = new KH2();
        MM11 mm11 = new MM11();
        WWHD wwhd = new WWHD();
        TY ty = new TY();
        public static System.Timers.Timer gameUpdater = new System.Timers.Timer(3000);
        public MainForm()
        {
            InitializeComponent();
            gameUpdater.Elapsed += new ElapsedEventHandler(gameUpdater_Tick);
            gameUpdater.Interval = 5000;
            gameUpdater.Enabled = true;

            Balloon("Beep Boop", "I am active now!");
            gameUpdater.Start();
        }
        private void gameUpdater_Tick(object sender, EventArgs e)
        {
            var game_kh1 = Process.GetProcessesByName("KINGDOM HEARTS FINAL MIX");
            var game_kh2 = Process.GetProcessesByName("KINGDOM HEARTS II FINAL MIX");
            var game_cemu = Process.GetProcessesByName("Cemu");
            var game_mm11 = Process.GetProcessesByName("game");
            var game_ty = Process.GetProcessesByName("TY");

            if (game_kh1.Length > 0)
            {
                Balloon("Found a game!", "I now keep track of Kingdom Hearts Final Mix.");
                kh1.DoAction();
                gameUpdater.Stop();
            }
            else if (game_kh2.Length > 0)
            {
                Balloon("Found a game!", "I now keep track of Kingdom Hearts II Final Mix.");
                kh2.DoAction();
                gameUpdater.Stop();
            }
            else if (game_mm11.Length > 0)
            {
                var title = Process.GetProcessesByName("game").FirstOrDefault();

                if (title.MainWindowTitle.Contains("MEGAMAN11"))
                {
                    Balloon("Found a game!", "I now keep track of Mega Man 11."); //Doing this because there might be other games with "game.exe"
                    mm11.DoAction();
                    gameUpdater.Stop();
                    return;
                }

            }
            else if (game_cemu.Length > 0)
            {
                var title = Process.GetProcessesByName("Cemu").FirstOrDefault();

                if (title.MainWindowTitle.Contains("Wind Waker HD"))
                {
                    Balloon("Found a game!", "I now keep track of Zelda: The Wind Waker HD");
                    wwhd.DoAction();
                    gameUpdater.Stop();
                    return;
                }
            }
            else if (game_ty.Length > 0)
            {
                Balloon("Found a game!", "I now keep track of TY the Tasmanian Tiger.");
                ty.DoAction();
                gameUpdater.Stop();
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Balloon(string title, string text)
        {
            notify.BalloonTipTitle = title;
            notify.BalloonTipText = text;
            notify.ShowBalloonTip(3000);
        }
    }
}
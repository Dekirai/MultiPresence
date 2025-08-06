using MultiPresenceGame.Presence;
using Steamworks;
using System.Timers;

namespace MultiPresenceGame
{
    public partial class MainForm : Form
    {
        public static System.Timers.Timer gameUpdater = new System.Timers.Timer(3000);
        public MainForm()
        {
            InitializeComponent();
#if DEBUG
            File.WriteAllText("steam_appid.txt", "440");
            if (!SteamAPI.Init())
            {
                //Do nothing
            }
            int keyCount = SteamFriends.GetFriendRichPresenceKeyCount(SteamUser.GetSteamID());

            if (keyCount == 0)
            {
                MessageBox.Show("No Rich Presence keys found.");
            }
            else
            {
                for (int i = 0; i < keyCount; i++)
                {
                    string key = SteamFriends.GetFriendRichPresenceKeyByIndex(SteamUser.GetSteamID(), i);
                    string value = SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), key);

                    MessageBox.Show($"Key: {key}, Value: {value}");
                }
            }
            gameUpdater.Elapsed += new ElapsedEventHandler(gameUpdater_Tick);
            gameUpdater.Interval = 5000;
            gameUpdater.Enabled = true;
            gameUpdater.Start();
#else
            gameUpdater.Elapsed += new ElapsedEventHandler(gameUpdater_Tick);
            gameUpdater.Interval = 5000;
            gameUpdater.Enabled = true;
            gameUpdater.Start();
#endif
        }

        private void gameUpdater_Tick(object sender, EventArgs e)
        {
            string game = GameDetector.GetGame();
            string json;

            switch (game)
            {
                case "Call of Duty®":
                    COD.DoAction();
                    gameUpdater.Stop();
                    break;
                case "Gunfire Reborn":
                    GFR.DoAction();
                    gameUpdater.Stop();
                    break;
                case "Hello Kitty Island Adventure":
                    HK.DoAction();
                    gameUpdater.Stop();
                    break;
                case "Hogwarts Legacy":
                    HL.DoAction();
                    gameUpdater.Stop();
                    break;
                case "Labyrinthine":
                    LR.DoAction();
                    gameUpdater.Stop();
                    break;
                case "Overwatch 2":
                    OW.DoAction();
                    gameUpdater.Stop();
                    break;
                case "Team Fortress 2":
                    TF2.DoAction();
                    gameUpdater.Stop();
                    break;
                case "Stellar Blade":
                    SB.DoAction();
                    gameUpdater.Stop();
                    break;
                case "Temtem: Swarm":
                    TTS.DoAction();
                    gameUpdater.Stop();
                    break;
            }
        }
    }
}

using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class TTS
    {

        public static void DoAction()
        {
            if (!File.Exists("steam_appid.txt"))
                File.WriteAllText("steam_appid.txt", "2510960");
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Combine the directory path with the file name
            string filePath = Path.Combine(currentDirectory, "MultiPresenceGame.exe");

            // Start the process
            Process.Start(filePath);
            Thread thread = new Thread(Blabla);
            thread.Start();
        }

        private static async void Blabla()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("TemtemSwarm");
                if (game.Length > 0)
                    await Task.Delay(300); // Wait before checking again
                else
                {
                    MainForm.gameUpdater.Start();
                    break;
                }
            }
        }
    }
}
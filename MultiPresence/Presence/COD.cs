using System.Diagnostics;
using System.Text.Json;

namespace MultiPresence.Presence
{
    public class COD
    {
        public static async Task DoAction()
        {
            await FileChecker.EnsureFilesExistAsync();

            if (!File.Exists("steam_appid.txt"))
                File.WriteAllText("steam_appid.txt", "1938090");

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string filePath = Path.Combine(currentDirectory, "MultiPresenceGame.exe");

            if (!File.Exists(filePath))
                return;

            Process.Start(filePath);
            Thread thread = new Thread(Blabla);
            thread.Start();
        }


        private static async void Blabla()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("cod");
                if (game.Length > 0)
                    await Task.Delay(3000); // Wait before checking again
                else
                {
                    MainForm.gameUpdater.Start();
                    break;
                }
            }
        }
    }
}
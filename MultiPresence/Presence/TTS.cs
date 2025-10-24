using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class TTS
    {
        public static async Task DoAction()
        {
            await FileChecker.EnsureFilesExistAsync();

            if (!File.Exists("Assets/steam_appid.txt"))
                File.WriteAllText("Assets/steam_appid.txt", "2510960");

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string file1 = Path.Combine(currentDirectory, "Assets/MultiPresenceGame.exe");
            string file2 = Path.Combine(currentDirectory, "Assets/steam_api64.dll");

            if (!File.Exists(file1) || !File.Exists(file2))
                return;

            Process.Start(file1);
            Thread thread = new Thread(Blabla);
            thread.Start();
        }

        private static async void Blabla()
        {
            Process[] game = Process.GetProcessesByName("TemtemSwarm");
            if (game.Length > 0)
                await Task.Delay(3000); // Wait before checking again
            else
            {
                MainForm.gameUpdater.Start();
            }
        }
    }
}
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class LR
    {
        public static async Task DoAction()
        {
            await FileChecker.EnsureFilesExistAsync();

            if (!File.Exists("steam_appid.txt"))
                File.WriteAllText("steam_appid.txt", "1302240");

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Define the files to check
            string file1 = Path.Combine(currentDirectory, "MultiPresenceGame.exe");
            string file2 = Path.Combine(currentDirectory, "steam_api64.dll");

            // Check if the files exist
            if (!File.Exists(file1) || !File.Exists(file2))
                return;

            Process.Start(file1);
            Thread thread = new Thread(Blabla);
            thread.Start();
        }

        private static async void Blabla()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("Labyrinthine");
                if (game.Length > 0)
                    await Task.Delay(1000); // Wait before checking again
                else
                {
                    MainForm.gameUpdater.Start();
                    break;
                }
            }
        }
    }
}
using Steamworks;
using System;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class OW
    {
        public static async void DoAction()
        {
            if (!File.Exists("steam_appid.txt"))
                File.WriteAllText("steam_appid.txt", "2357570");
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
                Process[] game = Process.GetProcessesByName("Overwatch");
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
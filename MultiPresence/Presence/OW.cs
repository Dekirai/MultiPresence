﻿using System.Diagnostics;

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

        }
    }
}
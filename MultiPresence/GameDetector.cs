using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPresence
{
    public static class GameDetector
    {
        public static int GetGame()
        {
            var game_kh1 = Process.GetProcessesByName("KINGDOM HEARTS FINAL MIX");
            var game_kh2 = Process.GetProcessesByName("KINGDOM HEARTS II FINAL MIX");
            var game_kh3 = Process.GetProcessesByName("KINGDOM HEARTS III");
            var game_khbbs = Process.GetProcessesByName("KINGDOM HEARTS Birth by Sleep FINAL MIX");
            var game_khddd = Process.GetProcessesByName("KINGDOM HEARTS Dream Drop Distance");
            var game_cemu = Process.GetProcessesByName("Cemu");
            var game_pcsx2 = Process.GetProcessesByName("pcsx2-qtx64-avx2");
            var game_mm11 = Process.GetProcessesByName("game");
            var game_re4 = Process.GetProcessesByName("bio4");
            var game_sa2 = Process.GetProcessesByName("sonic2app");
            var game_ty = Process.GetProcessesByName("TY");

            int game = 0;
            if (game_kh1.Length > 0)
                game = 1;
            else if (game_kh2.Length > 0)
                game = 2;
            else if (game_kh3.Length > 0)
                game = 3;
            else if (game_khbbs.Length > 0)
                game = 4;
            else if (game_khddd.Length > 0)
                game = 5;
            else if (game_mm11.Length > 0)
            {
                var title = Process.GetProcessesByName("game").FirstOrDefault();
                if (title.MainWindowTitle.Contains("MEGAMAN11"))
                    game = 6;
            }
            else if (game_re4.Length > 0)
                game = 7;
            else if (game_sa2.Length > 0)
                game = 8;
            else if (game_cemu.Length > 0)
            {
                var title = Process.GetProcessesByName("Cemu").FirstOrDefault();
                if (title.MainWindowTitle.Contains("Wind Waker HD"))
                    game = 9;
                else if (title.MainWindowTitle.Contains("Twilight Princess HD"))
                    game = 10;
            }
            else if (game_pcsx2.Length > 0)
            {
                var title = Process.GetProcessesByName("pcsx2-qtx64-avx2").FirstOrDefault();
                if (title.MainWindowTitle.Contains("Ratchet - Deadlocked"))
                    game = 11;
                else if (title.MainWindowTitle.Contains("Mega Man X - Command Mission"))
                    game = 12;
            }
            else if (game_ty.Length > 0)
                game = 13;

            return game;
        }
    }
}

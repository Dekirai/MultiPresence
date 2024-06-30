using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Memory;

namespace MultiPresence
{
    public static class GameDetector
    {
        static Mem mem = new Mem();
        public static string _cemu_titleid_address = "";
        public static string _cemu_titleid = "";
        public static bool _cemu_foundGame = false;

        public static async Task<int> GetGameAsync()
        {
            var game_kh1 = Process.GetProcessesByName("KINGDOM HEARTS FINAL MIX");
            var game_kh2 = Process.GetProcessesByName("KINGDOM HEARTS II FINAL MIX");
            var game_kh3 = Process.GetProcessesByName("KINGDOM HEARTS III");
            var game_khbbs = Process.GetProcessesByName("KINGDOM HEARTS Birth by Sleep FINAL MIX");
            var game_khddd = Process.GetProcessesByName("KINGDOM HEARTS Dream Drop Distance");
            var game_cemu = Process.GetProcessesByName("Cemu");
            var game_pcsx2 = Process.GetProcessesByName("pcsx2-qtx64-avx2");
            var game_mm11 = Process.GetProcessesByName("game");
            var game_re = Process.GetProcessesByName("bhd");
            var game_re4 = Process.GetProcessesByName("bio4");
            var game_re5 = Process.GetProcessesByName("re5dx9");
            var game_re6 = Process.GetProcessesByName("BH6");
            var game_rev2 = Process.GetProcessesByName("rerev2");
            var game_sa2 = Process.GetProcessesByName("sonic2app");
            var game_ty = Process.GetProcessesByName("TY");
            var game_pyre = Process.GetProcessesByName("ProjectG");

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
                await Task.Delay(15000); //Wait 15 Seconds 
                string pattern = @"TitleId:\s*([0-9a-fA-F-]+)";

                GetCemu();
                try
                {
                    long _gettitleid = (await mem.AoBScan("54 69 74 6C 65 49 64 3A 20 30 30 30 35 30 30 30 30 ?? ?? ?? ?? ?? ?? ?? ?? ?? 0D 0A 5B", true)).FirstOrDefault();
                    _cemu_titleid_address = _gettitleid.ToString("X11");
                    string _game = mem.ReadString($"{_cemu_titleid_address}");

                    Match match = Regex.Match(_game, pattern);
                    if (match.Success)
                    {
                        if (_cemu_foundGame == false)
                        {
                            string extractedPart = match.Groups[1].Value;
                            _cemu_titleid = extractedPart;
                            if (_cemu_titleid.Contains("10143600"))
                                game = 9; //Wind Waker HD EUR
                            else if (_cemu_titleid.Contains("10143599"))
                                game = 9; //Wind Waker HD USA Randomizer
                            else if (_cemu_titleid.Contains("10143500"))
                                game = 9; //Wind Waker HD USA
                            else if (_cemu_titleid.Contains("1019e600"))
                                game = 10; //Twilight Princess HD
                            _cemu_foundGame = true;
                        }
                    }
                    else
                        _cemu_foundGame = false;
                }
                catch
                {
                    _cemu_foundGame = false;
                }
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
            else if (game_re5.Length > 0)
                game = 14;
            else if (game_re6.Length > 0)
                game = 15;
            else if (game_re.Length > 0)
                game = 16;
            else if (game_rev2.Length > 0)
                game = 17;
            else if (game_pyre.Length > 0)
                game = 18;

            return game;
        }
        private static void GetCemu()
        {
            int pid = mem.GetProcIdFromName("Cemu");
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }
    }
}

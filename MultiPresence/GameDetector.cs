using Memory;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MultiPresence
{
    public static class GameDetector
    {
        static Mem mem = new Mem();
        public static string _cemu_titleid_address = "";
        public static string _cemu_titleid = "";
        public static bool _cemu_foundGame = false;

        public static async Task<string> GetGameAsync()
        {
            var game_asw = Process.GetProcessesByName("SoulWorker");
            var game_cemu = Process.GetProcessesByName("Cemu");
            var game_ccffvii = Process.GetProcessesByName("CCFF7R-Win64-Shipping");
            var game_ffviir = Process.GetProcessesByName("ff7remake_");
            var game_kh1 = Process.GetProcessesByName("KINGDOM HEARTS FINAL MIX");
            var game_kh2 = Process.GetProcessesByName("KINGDOM HEARTS II FINAL MIX");
            var game_kh3 = Process.GetProcessesByName("KINGDOM HEARTS III");
            var game_khbbs = Process.GetProcessesByName("KINGDOM HEARTS Birth by Sleep FINAL MIX");
            var game_khddd = Process.GetProcessesByName("KINGDOM HEARTS Dream Drop Distance");
            var game_mm11 = Process.GetProcessesByName("game");
            var game_mmbn6g = Process.GetProcessesByName("MMBN_LC2");
            var game_msmmm = Process.GetProcessesByName("MilesMorales");
            var game_msmr = Process.GetProcessesByName("Spider-Man");
            var game_ow = Process.GetProcessesByName("Overwatch");
            var game_pyre = Process.GetProcessesByName("ProjectG");
            var game_re = Process.GetProcessesByName("bhd");
            var game_re4 = Process.GetProcessesByName("bio4");
            var game_re5 = Process.GetProcessesByName("re5dx9");
            var game_re6 = Process.GetProcessesByName("BH6");
            var game_rev2 = Process.GetProcessesByName("rerev2");
            var game_sa2 = Process.GetProcessesByName("sonic2app");
            var game_ty = Process.GetProcessesByName("TY");

            string game = "";

            if (game_asw.Length > 0)
            {
                var title = Process.GetProcessesByName("SoulWorker").FirstOrDefault();
                if (title.MainWindowTitle.Contains("asobiSW"))
                {
                    game = "AsobiSW";
                }
            }
            else if (game_ccffvii.Length > 0)
                game = "CRISIS CORE –FINAL FANTASY VII– REUNION";
            else if (game_ffviir.Length > 0)
                game = "Final Fantasy VII Remake";
            else if (game_cemu.Length > 0)
            {
                string pattern = @"TitleId:\s*([0-9a-fA-F-]+)";

                GetCemu();
                try
                {
                    var title = Process.GetProcessesByName("Cemu").FirstOrDefault();
                    if (title.MainWindowTitle.Contains("TitleId"))
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
                                    game = "Zelda: The Wind Waker HD"; //Wind Waker HD EUR
                                else if (_cemu_titleid.Contains("10143599"))
                                    game = "Zelda: The Wind Waker HD"; //Wind Waker HD USA Randomizer
                                else if (_cemu_titleid.Contains("10143500"))
                                    game = "Zelda: The Wind Waker HD"; //Wind Waker HD USA
                                else if (_cemu_titleid.Contains("1019e500"))
                                    game = "Zelda: Twilight Princess HD"; //Twilight Princess HD USA
                                else if (_cemu_titleid.Contains("1019e600"))
                                    game = "Zelda: Twilight Princess HD"; //Twilight Princess HD EUR
                                _cemu_foundGame = true;
                            }
                        }
                        else
                            _cemu_foundGame = false;
                    }
                    else
                        _cemu_foundGame = false;
                }
                catch
                {
                    _cemu_foundGame = false;
                }
            }
            else if (game_kh1.Length > 0)
                game = "Kingdom Hearts Final Mix";
            else if (game_kh2.Length > 0)
                game = "Kingdom Hearts II Final Mix";
            else if (game_kh3.Length > 0)
                game = "Kingdom Hearts III";
            else if (game_khbbs.Length > 0)
                game = "Kingdom Hearts Birth by Sleep Final Mix";
            else if (game_khddd.Length > 0)
                game = "Kingdom Hearts Dream Drop Distance";
            else if (game_mm11.Length > 0)
            {
                var title = Process.GetProcessesByName("game").FirstOrDefault();
                if (title.MainWindowTitle.Contains("MEGAMAN11"))
                    game = "Mega Man 11";
            }
            else if (game_mmbn6g.Length > 0)
            {
                GetMMBNLC2();
                int _game = mem.ReadByte("MMBN_LC2.exe+ABEF0A0");
                if (_game == 9)
                    game = "Mega Man Battle Network 6: Cybeast Gregar";
                if (_game == 10)
                    game = "Mega Man Battle Network 6: Cybeast Falzar";
            }
            else if (game_msmmm.Length > 0)
                game = "Marvel's Spider-Man: Miles Morales";
            else if (game_msmr.Length > 0)
                game = "Marvel's Spider-Man Remastered";
            else if (game_ow.Length > 0)
                game = "Overwatch";
            else if (game_pyre.Length > 0)
                game = "Pangya Reborn";
            else if (game_re.Length > 0)
                game = "Resident Evil";
            else if (game_re4.Length > 0)
                game = "Resident Evil 4 (2005)";
            else if (game_re5.Length > 0)
                game = "Resident Evil 5";
            else if (game_re6.Length > 0)
                game = "Resident Evil 6";
            else if (game_rev2.Length > 0)
                game = "Resident Evil Revelations 2";
            else if (game_sa2.Length > 0)
                game = "Sonic Adventure 2";
            else if (game_ty.Length > 0)
                game = "TY the Tasmanian Tiger";

            return game;
        }
        private static void GetCemu()
        {
            int pid = mem.GetProcIdFromName("Cemu");
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }
        private static void GetMMBNLC2()
        {
            int pid = mem.GetProcIdFromName("MMBN_LC2");
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }
    }
}

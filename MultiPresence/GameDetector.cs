using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MultiPresence
{
    public static class GameDetector
    {

        public static ulong _cemu_titleid_address = 0;
        public static string _cemu_titleid = "";
        public static bool _cemu_foundGame = false;

        public static async Task<string> GetGameAsync()
        {
            var game_cemu = Process.GetProcessesByName("Cemu");
            var game_cod = Process.GetProcessesByName("cod");
            var game_ccffvii = Process.GetProcessesByName("CCFF7R-Win64-Shipping");
            var game_ffviir = Process.GetProcessesByName("ff7remake_");
            var game_ffviirb = Process.GetProcessesByName("ff7rebirth_");
            var game_ffxv = Process.GetProcessesByName("ffxv_s");
            var game_ffxvi = Process.GetProcessesByName("ffxvi");
            var game_dsr = Process.GetProcessesByName("DarkSoulsRemastered");
            var game_dsii = Process.GetProcessesByName("DarkSoulsII");
            var game_dsiii = Process.GetProcessesByName("DarkSoulsIII");
            var game_dmc = Process.GetProcessesByName("DMC-DevilMayCry");
            var game_dmc1 = Process.GetProcessesByName("dmc1");
            var game_dmc2 = Process.GetProcessesByName("dmc2");
            var game_dmc3 = Process.GetProcessesByName("dmc3");
            var game_dmc4 = Process.GetProcessesByName("DevilMayCry4SpecialEdition");
            var game_dmc5 = Process.GetProcessesByName("DevilMayCry5");
            var game_dmm = Process.GetProcessesByName("DivaMegaMix");
            var game_er = Process.GetProcessesByName("eldenring");
            var game_gfr = Process.GetProcessesByName("Gunfire Reborn");
            var game_hk = Process.GetProcessesByName("Hello Kitty");
            var game_hl = Process.GetProcessesByName("HogwartsLegacy");
            var game_kh1 = Process.GetProcessesByName("KINGDOM HEARTS FINAL MIX");
            var game_kh2 = Process.GetProcessesByName("KINGDOM HEARTS II FINAL MIX");
            var game_kh3 = Process.GetProcessesByName("KINGDOM HEARTS III");
            var game_khbbs = Process.GetProcessesByName("KINGDOM HEARTS Birth by Sleep FINAL MIX");
            var game_khddd = Process.GetProcessesByName("KINGDOM HEARTS Dream Drop Distance");
            var game_khcom = Process.GetProcessesByName("KINGDOM HEARTS Re_Chain of Memories");
            var game_lr = Process.GetProcessesByName("Labyrinthine");
            var game_lop = Process.GetProcessesByName("LOP-Win64-Shipping");
            var game_mm11 = Process.GetProcessesByName("game");
            var game_mmbn6g = Process.GetProcessesByName("MMBN_LC2");
            var game_msmmm = Process.GetProcessesByName("MilesMorales");
            var game_eac = Process.GetProcessesByName("EasyAntiCheat_EOS");
            var game_msmr = Process.GetProcessesByName("Spider-Man");
            var game_msm2 = Process.GetProcessesByName("Spider-Man2");
            var game_ow = Process.GetProcessesByName("Overwatch");
            var game_pyre = Process.GetProcessesByName("ProjectG");
            var game_re = Process.GetProcessesByName("bhd");
            var game_re2 = Process.GetProcessesByName("re2");
            var game_re4 = Process.GetProcessesByName("bio4");
            var game_re4r = Process.GetProcessesByName("re4");
            var game_re5 = Process.GetProcessesByName("re5dx9");
            var game_re6 = Process.GetProcessesByName("BH6");
            var game_rev2 = Process.GetProcessesByName("rerev2");
            var game_sadx = Process.GetProcessesByName("Sonic Adventure DX");
            var game_sa2 = Process.GetProcessesByName("sonic2app");
            var game_tts = Process.GetProcessesByName("TemtemSwarm");
            var game_ty = Process.GetProcessesByName("TY");
            var game_vs = Process.GetProcessesByName("VampireSurvivors");
            var game_vom = Process.GetProcessesByName("VisionsofMana-Win64-Shipping");

            string game = "";
            if (game_cod.Length > 0)
                game = "Call of Duty®";
            else if (game_gfr.Length > 0)
                game = "Gunfire Reborn";
            else if (game_ccffvii.Length > 0)
                game = "CRISIS CORE –FINAL FANTASY VII– REUNION";
            else if (game_dsr.Length > 0)
                game = "Dark Souls: Remastered";
            else if (game_dsii.Length > 0)
                game = "Dark Souls II";
            else if (game_dsiii.Length > 0)
                game = "Dark Souls III";
            else if (game_dmc.Length > 0)
                game = "DmC Devil May Cry";
            else if (game_dmc1.Length > 0)
                game = "Devil May Cry";
            else if (game_dmc2.Length > 0)
                game = "Devil May Cry 2";
            else if (game_dmc3.Length > 0)
                game = "Devil May Cry 3";
            else if (game_dmc4.Length > 0)
                game = "Devil May Cry 4";
            else if (game_dmc5.Length > 0)
                game = "Devil May Cry 5";
            else if (game_dmm.Length > 0)
                game = "Project Diva Mega Mix+";
            else if (game_ffviir.Length > 0)
                game = "Final Fantasy VII Remake";
            else if (game_ffviirb.Length > 0)
                game = "Final Fantasy VII Rebirth";
            else if (game_ffxv.Length > 0)
                game = "Final Fantasy XV";
            else if (game_ffxvi.Length > 0)
                game = "Final Fantasy XVI";
            else if (game_hk.Length > 0)
                game = "Hello Kitty Island Adventure";
            else if (game_hl.Length > 0)
                game = "Hogwarts Legacy";
            else if (game_cemu.Length > 0)
            {
                string pattern = @"TitleId:\s*([0-9a-fA-F-]+)";

                GetCemu();
                try
                {
                    var title = Process.GetProcessesByName("Cemu").FirstOrDefault();
                    if (title.MainWindowTitle.Contains("TitleId"))
                    {
                        _cemu_titleid_address = (ulong)Hypervisor.FindSignature("54 69 74 6C 65 49 64 3A 20 30 30 30 35 30 30 30 30 ?? ?? ?? ?? ?? ?? ?? ?? ?? 0D 0A 5B");
                        string _game = Hypervisor.ReadString(_cemu_titleid_address, 32, true);

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
            else if (game_khcom.Length > 0)
                game = "Kingdom Hearts Re:Chain of Memories";
            else if (game_lr.Length > 0)
                game = "Labyrinthine";
            else if (game_lop.Length > 0)
                game = "Lies of P";
            else if (game_mm11.Length > 0)
            {
                var title = Process.GetProcessesByName("game").FirstOrDefault();
                if (title.MainWindowTitle.Contains("MEGAMAN11"))
                    game = "Mega Man 11";
            }
            else if (game_mmbn6g.Length > 0)
            {
                GetMMBNLC2();
                int _game = Hypervisor.Read<byte>(0xABEF0A0);
                if (_game == 9)
                    game = "Mega Man Battle Network 6: Cybeast Gregar";
                if (_game == 10)
                    game = "Mega Man Battle Network 6: Cybeast Falzar";
            }
            else if (game_msmmm.Length > 0)
                game = "Marvel's Spider-Man: Miles Morales";
            else if (game_msmr.Length > 0)
                game = "Marvel's Spider-Man Remastered";
            else if (game_msm2.Length > 0)
                game = "Marvel's Spider-Man 2";
            else if (game_ow.Length > 0)
                game = "Overwatch 2";
            else if (game_pyre.Length > 0)
                game = "Pangya Reborn";
            else if (game_re.Length > 0)
                game = "Resident Evil";
            else if (game_re2.Length > 0)
                game = "Resident Evil 2";
            else if (game_re4.Length > 0)
                game = "Resident Evil 4 (2005)";
            else if (game_re4r.Length > 0)
                game = "Resident Evil 4 Remake";
            else if (game_re5.Length > 0)
                game = "Resident Evil 5";
            else if (game_re6.Length > 0)
                game = "Resident Evil 6";
            else if (game_rev2.Length > 0)
                game = "Resident Evil Revelations 2";
            else if (game_sa2.Length > 0)
                game = "Sonic Adventure 2";
            else if (game_sadx.Length > 0)
                game = "Sonic Adventure DX";
            else if (game_tts.Length > 0)
                game = "Temtem: Swarm";
            else if (game_ty.Length > 0)
                game = "TY the Tasmanian Tiger";
            else if (game_vs.Length > 0)
                game = "Vampire Survivors";
            else if (game_vom.Length > 0)
                game = "Visions of Mana";
            else if (game_er.Length > 0)
            {
                if (game_eac.Length == 0)
                    game = "Elden Ring";
            }

            return game;
        }
        private static void GetCemu()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("Cemu")[0];
                if (_myProcess.Id > 0)
                    Hypervisor.AttachProcess(_myProcess);
            }
            catch
            {
                //nothing?
            }
        }
        private static void GetMMBNLC2()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("MMBN_LC2")[0];
                if (_myProcess.Id > 0)
                    Hypervisor.AttachProcess(_myProcess);
            }
            catch
            {
                //nothing?
            }
        }
    }
}

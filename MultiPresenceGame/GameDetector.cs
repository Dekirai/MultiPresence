using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MultiPresenceGame
{
    public static class GameDetector
    {
        public static async Task<string> GetGameAsync()
        {
            var game_cod = Process.GetProcessesByName("cod");
            var game_gfr = Process.GetProcessesByName("Gunfire Reborn");
            var game_hk = Process.GetProcessesByName("Hello Kitty");
            var game_hl = Process.GetProcessesByName("HogwartsLegacy");
            var game_lr = Process.GetProcessesByName("Labyrinthine");
            var game_ow = Process.GetProcessesByName("Overwatch");
            var game_sb = Process.GetProcessesByName("SB-Win64-Shipping");
            var game_tts = Process.GetProcessesByName("TemtemSwarm");

            string game = "";
            if (game_cod.Length > 0)
                game = "Call of Duty®";
            else if (game_gfr.Length > 0)
                game = "Gunfire Reborn";
            else if (game_hk.Length > 0)
                game = "Hello Kitty Island Adventure";
            else if (game_hl.Length > 0)
                game = "Hogwarts Legacy";
            else if (game_lr.Length > 0)
                game = "Labyrinthine";
            else if (game_ow.Length > 0)
                game = "Overwatch 2";
            else if (game_sb.Length > 0)
                game = "Stellar Blade";
            else if (game_tts.Length > 0)
                game = "Temtem: Swarm";

            return game;
        }
    }
}

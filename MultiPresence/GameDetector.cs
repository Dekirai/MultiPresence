using MultiPresence.Core;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MultiPresence
{
    public static class GameDetector
    {
        private static readonly Dictionary<string, string> GameProcessMap = new(StringComparer.OrdinalIgnoreCase)
        {
            {"BorderlandsGOTY", "Borderlands 1"}, {"Borderlands2", "Borderlands 2"}, {"cod", "Call of Duty®"},
            {"CrashBandicoot4", "Crash Bandicoot 4: It's About Time"}, {"CrashBandicootNSaneTrilogy", "Crash Bandicoot N. Sane Trilogy"},
            {"Gunfire Reborn", "Gunfire Reborn"}, {"CCFF7R-Win64-Shipping", "CRISIS CORE –FINAL FANTASY VII– REUNION"},
            {"DarkSoulsRemastered", "Dark Souls: Remastered"}, {"DarkSoulsII", "Dark Souls II"}, {"DarkSoulsIII", "Dark Souls III"},
            {"ds", "Death Stranding"}, {"Diablo IV", "Diablo IV"}, {"DMC-DevilMayCry", "DmC Devil May Cry"},
            {"dmc1", "Devil May Cry"}, {"dmc2", "Devil May Cry 2"}, {"dmc3", "Devil May Cry 3"},
            {"DevilMayCry4SpecialEdition", "Devil May Cry 4"}, {"DevilMayCry5", "Devil May Cry 5"},
            {"DivaMegaMix", "Project Diva Mega Mix+"}, {"ff7remake_", "Final Fantasy VII Remake"}, {"ff7rebirth_", "Final Fantasy VII Rebirth"},
            {"ffx", "Final Fantasy X"}, {"ffxv_s", "Final Fantasy XV"}, {"ffxvi", "Final Fantasy XVI"}, {"CodeVein-Win64-Shipping", "CODE VEIN"},
            {"Digimon Story Time Stranger", "Digimon Story Time Stranger"}, {"granblue_fantasy_relink", "Granblue Fantasy: Relink"},
            {"Hello Kitty", "Hello Kitty Island Adventure"}, {"HogwartsLegacy", "Hogwarts Legacy"},
            {"KINGDOM HEARTS FINAL MIX", "Kingdom Hearts Final Mix"}, {"KINGDOM HEARTS II FINAL MIX", "Kingdom Hearts II Final Mix"},
            {"KINGDOM HEARTS III", "Kingdom Hearts III"}, {"KINGDOM HEARTS Birth by Sleep FINAL MIX", "Kingdom Hearts Birth by Sleep Final Mix"},
            {"KINGDOM HEARTS Dream Drop Distance", "Kingdom Hearts Dream Drop Distance"}, {"KINGDOM HEARTS Re_Chain of Memories", "Kingdom Hearts Re:Chain of Memories"},
            {"Labyrinthine", "Labyrinthine"}, {"LOP-Win64-Shipping", "Lies of P"}, {"game", "Multiple Games"},
            {"MilesMorales", "Marvel's Spider-Man: Miles Morales"}, {"Spider-Man", "Marvel's Spider-Man Remastered"},
            {"Overwatch", "Overwatch"}, {"tf_win64", "Team Fortress 2"}, {"ProjectG", "Pangya Reborn"},
            {"p4g", "Persona 4 Golden"}, {"P5R", "Persona 5 Royal"}, {"P5S", "Persona 5 Strikers"}, {"Persona 5 Tactica", "Persona 5 Tactica"},
            {"P5X", "Persona 5: The Phantom X"}, {"rayman30th", "Rayman"}, {"bhd", "Resident Evil"}, {"re2", "Resident Evil 2"},
            {"re3", "Resident Evil 3"}, {"bio4", "Resident Evil 4 (2005)"}, {"re4", "Resident Evil 4 Remake"}, {"re5dx9", "Resident Evil 5"},
            {"BH6", "Resident Evil 6"}, {"re7", "Resident Evil 7"}, {"re8", "Resident Evil 8"}, {"rerev2", "Resident Evil Revelations 2"},
            {"scott", "Scott Pilgrim vs The World"}, {"smt3hd", "Shin Megami Tensei III"}, {"SMT5V-Win64-Shipping", "Shin Megami Tensei V"},
            {"sonic2app", "Sonic Adventure 2"}, {"Sonic Adventure DX", "Sonic Adventure DX"}, {"SONIC_GENERATIONS", "Sonic Generations"},
            {"SB-Win64-Shipping", "Stellar Blade"}, {"TemtemSwarm", "Temtem: Swarm"}, {"isaac-ng", "The Binding of Isaac: Rebirth"},
            {"witcher3", "The Witcher 3"}, {"TY", "TY the Tasmanian Tiger"}, {"VampireSurvivors", "Vampire Survivors"},
            {"VisionsofMana-Win64-Shipping", "Visions of Mana"}, {"ys1plus", "Ys I Chronicles"}
        };

        private static readonly Dictionary<string, string> CemuTitleMap = new(StringComparer.OrdinalIgnoreCase)
        {
            {"10143600", "Zelda: The Wind Waker HD"}, {"10143599", "Zelda: The Wind Waker HD"}, {"10143500", "Zelda: The Wind Waker HD"},
            {"1019e500", "Zelda: Twilight Princess HD"}, {"1019e600", "Zelda: Twilight Princess HD"}
        };

        public static string GetGame()
        {
            Process[] snapshot = [];
            try
            {
                snapshot = Process.GetProcesses();
                var processes = new Dictionary<string, Process>(StringComparer.OrdinalIgnoreCase);
                foreach (var process in snapshot)
                    processes.TryAdd(process.ProcessName, process);

                foreach (var (processName, title) in GameProcessMap)
                {
                    if (!processes.TryGetValue(processName, out var process))
                        continue;

                    return processName.Equals("game", StringComparison.OrdinalIgnoreCase)
                        ? DetectGameTitle(process) ?? title
                        : title;
                }

                if (processes.TryGetValue("Cemu", out var cemu))
                    return DetectCemuGame(cemu) ?? string.Empty;

                if (processes.TryGetValue("MMBN_LC1", out var mmbn1))
                    return DetectMmbn1Game(mmbn1) ?? string.Empty;

                if (processes.TryGetValue("MMBN_LC2", out var mmbn2))
                    return DetectMmbnGame(mmbn2) ?? string.Empty;

                if (processes.TryGetValue("RXC2", out var mmx))
                    return DetectMmXGame(mmx) ?? string.Empty;

                if (processes.ContainsKey("eldenring") && !processes.ContainsKey("EasyAntiCheat_EOS"))
                    return "Elden Ring";

                return string.Empty;
            }
            catch (Exception ex)
            {
                RateLimitedLogger.Error("game-detection", ex);
                return string.Empty;
            }
            finally
            {
                foreach (var process in snapshot)
                    process.Dispose();
            }
        }

        private static string? DetectCemuGame(Process cemu)
        {
            try
            {
                Hypervisor.AttachProcess(cemu);
                if (!cemu.MainWindowTitle.Contains("TitleId", StringComparison.Ordinal))
                    return null;

                var address = (ulong)Hypervisor.FindSignature("54 69 74 6C 65 49 64 3A 20 30 30 30 35 30 30 30 30 ?? ?? ?? ?? ?? ?? ?? ?? ?? 0D 0A 5B");
                var data = Hypervisor.ReadString(address, 32, true);
                var match = Regex.Match(data, @"TitleId:\s*([0-9a-fA-F]+)");
                return match.Success && CemuTitleMap.TryGetValue(match.Groups[1].Value, out var game) ? game : null;
            }
            catch (Exception ex)
            {
                RateLimitedLogger.Error("detect-cemu", ex);
                return null;
            }
        }

        private static string? DetectMmbn1Game(Process process)
        {
            try
            {
                Hypervisor.AttachProcess(process);
                return Hypervisor.Read<byte>(0x987499C) switch
                {
                    0 => "Mega Man Battle Network", 1 => "Mega Man Battle Network 2", 2 => "Mega Man Battle Network 3", _ => null
                };
            }
            catch (Exception ex)
            {
                RateLimitedLogger.Error("detect-mmbn1", ex);
                return null;
            }
        }

        private static string? DetectMmbnGame(Process process)
        {
            try
            {
                Hypervisor.AttachProcess(process);
                return Hypervisor.Read<byte>(0xABEF0A0) switch
                {
                    5 or 6 => "Mega Man Battle Network 4", 7 or 8 => "Mega Man Battle Network 5", 9 or 10 => "Mega Man Battle Network 6", _ => null
                };
            }
            catch (Exception ex)
            {
                RateLimitedLogger.Error("detect-mmbn2", ex);
                return null;
            }
        }

        private static string? DetectMmXGame(Process process)
        {
            try
            {
                Hypervisor.AttachProcess(process);
                return Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x0338ED04, [0x90]), true) switch
                {
                    0 => "Mega Man X5", 1 => "Mega Man X6", 2 => "Mega Man X7", 3 => "Mega Man X8", _ => null
                };
            }
            catch (Exception ex)
            {
                RateLimitedLogger.Error("detect-mmx", ex);
                return null;
            }
        }

        private static string? DetectGameTitle(Process process)
        {
            try
            {
                var title = process.MainWindowTitle;
                return title.Contains("MEGAMAN11", StringComparison.OrdinalIgnoreCase) ? "Mega Man 11"
                    : title.Contains("Persona 5 Strikers", StringComparison.OrdinalIgnoreCase) ? "Persona 5 Strikers"
                    : null;
            }
            catch (Exception ex)
            {
                RateLimitedLogger.Error("detect-window-title", ex);
                return null;
            }
        }
    }
}

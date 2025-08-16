using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace MultiPresence
{
    public static class GameDetector
    {
        // Mapping process names to game titles
        // {"processname", "Game Title"}

        private static readonly Dictionary<string, string> GameProcessMap = new(StringComparer.OrdinalIgnoreCase)
        {
            {"cod", "Call of Duty®"},
            {"CrashBandicoot4", "Crash Bandicoot 4: It's About Time"},
            {"CrashBandicootNSaneTrilogy", "Crash Bandicoot N. Sane Trilogy"},
            {"Gunfire Reborn", "Gunfire Reborn"},
            {"CCFF7R-Win64-Shipping", "CRISIS CORE –FINAL FANTASY VII– REUNION"},
            {"DarkSoulsRemastered", "Dark Souls: Remastered"},
            {"DarkSoulsII", "Dark Souls II"},
            {"DarkSoulsIII", "Dark Souls III"},
            {"ds", "Death Stranding" },
            {"DMC-DevilMayCry", "DmC Devil May Cry"},
            {"dmc1", "Devil May Cry"},
            {"dmc2", "Devil May Cry 2"},
            {"dmc3", "Devil May Cry 3"},
            {"DevilMayCry4SpecialEdition", "Devil May Cry 4"},
            {"DevilMayCry5", "Devil May Cry 5"},
            {"DivaMegaMix", "Project Diva Mega Mix+"},
            {"ff7remake_", "Final Fantasy VII Remake"},
            {"ff7rebirth_", "Final Fantasy VII Rebirth"},
            {"ffxv_s", "Final Fantasy XV"},
            {"ffxvi", "Final Fantasy XVI"},
            {"CodeVein-Win64-Shipping", "CODE VEIN"},
            {"Hello Kitty", "Hello Kitty Island Adventure"},
            {"HogwartsLegacy", "Hogwarts Legacy"},
            {"KINGDOM HEARTS FINAL MIX", "Kingdom Hearts Final Mix"},
            {"KINGDOM HEARTS II FINAL MIX", "Kingdom Hearts II Final Mix"},
            {"KINGDOM HEARTS III", "Kingdom Hearts III"},
            {"KINGDOM HEARTS Birth by Sleep FINAL MIX", "Kingdom Hearts Birth by Sleep Final Mix"},
            {"KINGDOM HEARTS Dream Drop Distance", "Kingdom Hearts Dream Drop Distance"},
            {"KINGDOM HEARTS Re_Chain of Memories", "Kingdom Hearts Re:Chain of Memories"},
            {"Labyrinthine", "Labyrinthine"},
            {"LOP-Win64-Shipping", "Lies of P"},
            {"game", "Multiple Games"},
            {"MilesMorales", "Marvel's Spider-Man: Miles Morales"},
            {"Spider-Man", "Marvel's Spider-Man Remastered"},
            {"Spider-Man2", "Marvel's Spider-Man 2"},
            {"Overwatch", "Overwatch 2"},
            {"tf_win64", "Team Fortress 2"},
            {"ProjectG", "Pangya Reborn"},
            {"p4g", "Persona 4 Golden"},
            {"P5R", "Persona 5 Royal"},
            {"P5S", "Persona 5 Strikers"},
            {"P5X", "Persona 5: The Phantom X"},
            {"bhd", "Resident Evil"},
            {"re2", "Resident Evil 2"},
            {"bio4", "Resident Evil 4 (2005)"},
            {"re4", "Resident Evil 4 Remake"},
            {"re5dx9", "Resident Evil 5"},
            {"BH6", "Resident Evil 6"},
            {"re7", "Resident Evil 7"},
            {"re8", "Resident Evil 8" },
            {"rerev2", "Resident Evil Revelations 2"},
            {"scott", "Scott Pilgrim vs The World"},
            {"sonic2app", "Sonic Adventure 2"},
            {"Sonic Adventure DX", "Sonic Adventure DX"},
            {"SB-Win64-Shipping", "Stellar Blade"},
            {"TemtemSwarm", "Temtem: Swarm"},
            {"isaac-ng", "The Binding of Isaac: Rebirth"},
            {"witcher3", "The Witcher 3"},
            {"TY", "TY the Tasmanian Tiger"},
            {"VampireSurvivors", "Vampire Survivors"},
            {"VisionsofMana-Win64-Shipping", "Visions of Mana"}
        };

        // Mapping Cemu Title IDs to game titles
        private static readonly Dictionary<string, string> CemuTitleMap = new(StringComparer.OrdinalIgnoreCase)
        {
            {"10143600", "Zelda: The Wind Waker HD"},
            {"10143599", "Zelda: The Wind Waker HD"},
            {"10143500", "Zelda: The Wind Waker HD"},
            {"1019e500", "Zelda: Twilight Princess HD"},
            {"1019e600", "Zelda: Twilight Princess HD"}
        };

        public static string GetGame()
        {
            var processes = Process
                            .GetProcesses()
                            .DistinctBy(p => p.ProcessName, StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(
                            p => p.ProcessName,
                            StringComparer.OrdinalIgnoreCase
            );

            foreach (var kvp in GameProcessMap)
            {
                if (processes.ContainsKey(kvp.Key))
                {
                    // Special-case 'game' for Mega Man 11, Persona 5 Strikers, etc.
                    return kvp.Key == "game"
                        ? DetectGameTitle(processes["game"]) ?? kvp.Value
                        : kvp.Value;
                }
            }

            if (processes.ContainsKey("Cemu"))
            {
                var cemuTitle = DetectCemuGame(processes["Cemu"]);
                if (!string.IsNullOrEmpty(cemuTitle))
                    return cemuTitle;
            }

            if (processes.ContainsKey("MMBN_LC2"))
            {
                var mmbn = DetectMmbnGame(processes["MMBN_LC2"]);
                if (!string.IsNullOrEmpty(mmbn))
                    return mmbn;
            }

            // Elden Ring without EAC
            if (processes.ContainsKey("eldenring") && !processes.ContainsKey("EasyAntiCheat_EOS"))
                return "Elden Ring";

            return string.Empty;
        }

        private static string? DetectCemuGame(Process cemu)
        {
            try
            {
                Hypervisor.AttachProcess(cemu);
                if (!cemu.MainWindowTitle.Contains("TitleId"))
                    return null;

                var address = (ulong)Hypervisor.FindSignature(
                    "54 69 74 6C 65 49 64 3A 20 30 30 30 35 30 30 30 30 ?? ?? ?? ?? ?? ?? ?? ?? ?? 0D 0A 5B");
                var data = Hypervisor.ReadString(address, 32, true);
                var match = Regex.Match(data, @"TitleId:\s*([0-9a-fA-F]+)");
                return (match.Success && CemuTitleMap.TryGetValue(match.Groups[1].Value, out var game))
                    ? game : null;
            }
            catch
            {
                return null;
            }
        }

        private static string? DetectMmbnGame(Process mmbn)
        {
            try
            {
                Hypervisor.AttachProcess(mmbn);
                int code = Hypervisor.Read<byte>(0xABEF0A0);
                return code switch
                {
                    9 => "Mega Man Battle Network 6: Cybeast Gregar",
                    10 => "Mega Man Battle Network 6: Cybeast Falzar",
                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }

        private static string? DetectGameTitle(Process gameProcess)
        {
            var title = gameProcess.MainWindowTitle;
            return title.Contains("MEGAMAN11") ? "Mega Man 11"
                 : title.Contains("Persona 5 Strikers") ? "Persona 5 Strikers"
                 : null;
        }
    }
}

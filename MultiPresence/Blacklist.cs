using Newtonsoft.Json;

namespace MultiPresence
{
    public class Blacklist
    {
        [JsonProperty("Call of Duty®")]
        public bool CallofDuty { get; set; }
        [JsonProperty("CRISIS CORE –FINAL FANTASY VII– REUNION")]
        public bool CrisisCoreFinalFantasyVIIReunion { get; set; }
        [JsonProperty("The Binding of Isaac: Rebirth")]
        public bool TheBindingOfIsaacRebirth { get; set; }
        [JsonProperty("Crash Bandicoot 4: It's About Time")]
        public bool CrashBandicoot4 { get; set; }
        [JsonProperty("Crash Bandicoot N. Sane Trilogy")]
        public bool CrashBandicootNSaneTrilogy { get; set; }
        [JsonProperty("CODE VEIN")]
        public bool CodeVein { get; set; }
        [JsonProperty("Dark Souls: Remastered")]
        public bool DarkSoulsRemastered { get; set; }
        [JsonProperty("Dark Souls II")]
        public bool DarkSoulsII { get; set; }
        [JsonProperty("Dark Souls III")]
        public bool DarkSoulsIII { get; set; }
        [JsonProperty("Death Stranding")]
        public bool DeathStranding { get; set; }
        [JsonProperty("Devil May Cry")]
        public bool DevilMayCry { get; set; }
        [JsonProperty("Devil May Cry 2")]
        public bool DevilMayCry2 { get; set; }
        [JsonProperty("Devil May Cry 3")]
        public bool DevilMayCry3 { get; set; }
        [JsonProperty("Devil May Cry 4")]
        public bool DevilMayCry4 { get; set; }
        [JsonProperty("Devil May Cry 5")]
        public bool DevilMayCry5 { get; set; }
        [JsonProperty("DmC Devil May Cry")]
        public bool DmCDevilMayCry { get; set; }
        [JsonProperty("Elden Ring")]
        public bool EldenRing { get; set; }
        [JsonProperty("Final Fantasy VII Rebirth")]
        public bool FinalFantasyVIIRebirth { get; set; }
        [JsonProperty("Final Fantasy VII Remake")]
        public bool FinalFantasyVIIRemake { get; set; }
        [JsonProperty("Final Fantasy XVI")]
        public bool FinalFantasyXVI { get; set; }
        [JsonProperty("Final Fantasy XV")]
        public bool FinalFantasyXV { get; set; }
        [JsonProperty("Gunfire Reborn")]
        public bool GunfireReborn { get; set; }
        [JsonProperty("Project Diva Mega Mix+")]
        public bool ProjectDivaMegaMixPlus { get; set; }
        [JsonProperty("Kingdom Hearts Birth by Sleep Final Mix")]
        public bool KingdomHeartsBirthBySleepFinalMix { get; set; }
        [JsonProperty("Kingdom Hearts Re:Chain of Memories")]
        public bool KingdomHeartsReChainofMemories { get; set; }
        [JsonProperty("Kingdom Hearts Dream Drop Distance")]
        public bool KingdomHeartsDreamDropDistance { get; set; }
        [JsonProperty("Kingdom Hearts Final Mix")]
        public bool KingdomHeartsFinalMix { get; set; }
        [JsonProperty("Kingdom Hearts II Final Mix")]
        public bool KingdomHeartsIIFinalMix { get; set; }
        [JsonProperty("Kingdom Hearts III")]
        public bool KingdomHeartsIII { get; set; }
        [JsonProperty("Labyrinthine")]
        public bool Labyrinthine { get; set; }
        [JsonProperty("Lies of P")]
        public bool LiesofP { get; set; }
        [JsonProperty("Marvel's Spider-Man Remastered")]
        public bool MarvelsSpiderManRemastered { get; set; }
        [JsonProperty("Marvel's Spider-Man 2")]
        public bool MarvelsSpiderMan2 { get; set; }
        [JsonProperty("Marvel's Spider-Man: Miles Morales")]
        public bool MarvelsSpiderManMilesMorales { get; set; }
        [JsonProperty("Mega Man 11")]
        public bool MegaMan11 { get; set; }
        [JsonProperty("Mega Man Battle Network 6: Cybeast Gregar")]
        public bool MegaManBattleNetwork6CybeastGregar { get; set; }
        [JsonProperty("Mega Man Battle Network 6: Cybeast Falzar")]
        public bool MegaManBattleNetwork6CybeastFalzar { get; set; }
        [JsonProperty("Overwatch 2")]
        public bool Overwatch2 { get; set; }
        [JsonProperty("Pangya Reborn")]
        public bool PangyaReborn { get; set; }
        [JsonProperty("Persona 4 Golden")]
        public bool P4G { get; set; }
        [JsonProperty("Persona 5 Strikers")]
        public bool P5S { get; set; }
        [JsonProperty("Persona 5 Royal")]
        public bool P5R { get; set; }
        [JsonProperty("Persona 5: The Phantom X")]
        public bool P5X { get; set; }
        [JsonProperty("Resident Evil")]
        public bool ResidentEvil { get; set; }
        [JsonProperty("Resident Evil 2")]
        public bool ResidentEvil2 { get; set; }
        [JsonProperty("Resident Evil 4 (2005)")]
        public bool ResidentEvil42005 { get; set; }
        [JsonProperty("Resident Evil 4 Remake")]
        public bool ResidentEvil4Remake { get; set; }
        [JsonProperty("Resident Evil 5")]
        public bool ResidentEvil5 { get; set; }
        [JsonProperty("Resident Evil 6")]
        public bool ResidentEvil6 { get; set; }
        [JsonProperty("Resident Evil 7")]
        public bool ResidentEvil7 { get; set; }
        [JsonProperty("Resident Evil 8")]
        public bool ResidentEvil8 { get; set; }
        [JsonProperty("Resident Evil Revelations 2")]
        public bool ResidentEvilRevelations2 { get; set; }
        [JsonProperty("Scott Pilgrim vs The World")]
        public bool Scott { get; set; }
        [JsonProperty("Sonic Adventure 2")]
        public bool SonicAdventure2 { get; set; }
        [JsonProperty("Sonic Adventure DX")]
        public bool SonicAdventureDX { get; set; }
        [JsonProperty("Stellar Blade")]
        public bool StellarBlade { get; set; }
        [JsonProperty("Team Fortress 2")]
        public bool TeamFortress2 { get; set; }
        [JsonProperty("Temtem: Swarm")]
        public bool TemtemSwarm { get; set; }
        [JsonProperty("The Witcher 3")]
        public bool TheWitcher3 { get; set; }
        [JsonProperty("TY the Tasmanian Tiger")]
        public bool TYTheTasmanianTiger { get; set; }
        [JsonProperty("Zelda: The Wind Waker HD")]
        public bool ZeldaTheWindWakerHD { get; set; }
        [JsonProperty("Zelda: Twilight Princess HD")]
        public bool ZeldaTwilightPrincessHD { get; set; }
        [JsonProperty("Vampire Survivors")]
        public bool VampireSurvivors { get; set; }
        [JsonProperty("Visions of Mana")]
        public bool VisionsOfMana { get; set; }

        public bool GetValue(string gameTitle)
        {
            switch (gameTitle)
            {
                case "Call of Duty®":
                    return CallofDuty;
                case "Crash Bandicoot 4: It's About Time":
                    return CrashBandicoot4;
                case "Crash Bandicoot N. Sane Trilogy":
                    return CrashBandicootNSaneTrilogy;
                case "CRISIS CORE –FINAL FANTASY VII– REUNION":
                    return CrisisCoreFinalFantasyVIIReunion;
                case "CODE VEIN":
                    return CodeVein;
                case "Dark Souls: Remastered":
                    return DarkSoulsRemastered;
                case "Dark Souls II":
                    return DarkSoulsII;
                case "Dark Souls III":
                    return DarkSoulsIII;
                case "Death Stranding":
                    return DeathStranding;
                case "Devil May Cry ":
                    return DevilMayCry;
                case "Devil May Cry 2":
                    return DevilMayCry2;
                case "Devil May Cry 3":
                    return DevilMayCry3;
                case "Devil May Cry 4":
                    return DevilMayCry4;
                case "Devil May Cry 5":
                    return DevilMayCry5;
                case "Elden Ring":
                    return EldenRing;
                case "Final Fantasy VII Rebirth":
                    return FinalFantasyVIIRebirth;
                case "Final Fantasy VII Remake":
                    return FinalFantasyVIIRemake;
                case "Final Fantasy XV":
                    return FinalFantasyXV;
                case "Final Fantasy XVI":
                    return FinalFantasyXVI;
                case "Gunfire Reborn":
                    return GunfireReborn;
                case "Kingdom Hearts Birth by Sleep Final Mix":
                    return KingdomHeartsBirthBySleepFinalMix;
                case "Kingdom Hearts Re:Chain of Memories":
                    return KingdomHeartsReChainofMemories;
                case "Kingdom Hearts Dream Drop Distance":
                    return KingdomHeartsDreamDropDistance;
                case "Kingdom Hearts Final Mix":
                    return KingdomHeartsFinalMix;
                case "Kingdom Hearts II Final Mix":
                    return KingdomHeartsIIFinalMix;
                case "Kingdom Hearts III":
                    return KingdomHeartsIII;
                case "Labyrinthine":
                    return Labyrinthine;
                case "Lies of P":
                    return LiesofP;
                case "Marvel's Spider-Man Remastered":
                    return MarvelsSpiderManRemastered;
                case "Marvel's Spider-Man 2":
                    return MarvelsSpiderMan2;
                case "Marvel's Spider-Man: Miles Morales":
                    return MarvelsSpiderManMilesMorales;
                case "Mega Man 11":
                    return MegaMan11;
                case "Mega Man Battle Network 6: Cybeast Gregar":
                    return MegaManBattleNetwork6CybeastGregar;
                case "Mega Man Battle Network 6: Cybeast Falzar":
                    return MegaManBattleNetwork6CybeastFalzar;
                case "Overwatch 2":
                    return Overwatch2;
                case "Pangya Reborn":
                    return PangyaReborn;
                case "Persona 5 Strikers":
                    return P5S;
                case "Persona 5 Royal":
                    return P5R;
                case "Persona 5: The Phantom X":
                    return P5X;
                case "Project Diva Mega Mix+":
                    return ProjectDivaMegaMixPlus;
                case "Resident Evil":
                    return ResidentEvil;
                case "Resident Evi 2":
                    return ResidentEvil2;
                case "Resident Evil 4 (2005)":
                    return ResidentEvil42005;
                case "Resident Evil 4 Remake":
                    return ResidentEvil4Remake;
                case "Resident Evil 5":
                    return ResidentEvil5;
                case "Resident Evil 6":
                    return ResidentEvil6;
                case "Resident Evil 7":
                    return ResidentEvil7;
                case "Resident Evil 8":
                    return ResidentEvil8;
                case "Resident Evil Revelations 2":
                    return ResidentEvilRevelations2;
                case "Scott Pilgrim vs The World":
                    return Scott;
                case "Sonic Adventure 2":
                    return SonicAdventure2;
                case "Sonic Adventure DX":
                    return SonicAdventureDX;
                case "Stellar Blade":
                    return StellarBlade;
                case "Team Fortress 2":
                    return TeamFortress2;
                case "Temtem: Swarm":
                    return TemtemSwarm;
                case "The Binding of Isaac: Rebirth":
                    return TheBindingOfIsaacRebirth;
                case "TY the Tasmanian Tiger":
                    return TYTheTasmanianTiger;
                case "Zelda: The Wind Waker HD":
                    return ZeldaTheWindWakerHD;
                case "Zelda: Twilight Princess HD":
                    return ZeldaTwilightPrincessHD;
                case "Vampire Survivors":
                    return VampireSurvivors;
                case "Visions of Mana":
                    return VisionsOfMana;
                default:
                    return false;
            }
        }

    }
}

using Newtonsoft.Json;

namespace MultiPresence
{
    public class Blacklist
    {
        [JsonProperty("Call of Duty®")]
        public bool CallofDuty { get; set; }
        [JsonProperty("CRISIS CORE –FINAL FANTASY VII– REUNION")]
        public bool CrisisCoreFinalFantasyVIIReunion { get; set; }
        [JsonProperty("Dark Souls: Remastered")]
        public bool DarkSoulsRemastered { get; set; }
        [JsonProperty("Dark Souls II")]
        public bool DarkSoulsII { get; set; }
        [JsonProperty("Dark Souls III")]
        public bool DarkSoulsIII { get; set; }
        [JsonProperty("Elden Ring")]
        public bool EldenRing { get; set; }
        [JsonProperty("Final Fantasy VII Rebirth")]
        public bool FinalFantasyVIIRebirth { get; set; }
        [JsonProperty("Final Fantasy VII Remake")]
        public bool FinalFantasyVIIRemake { get; set; }
        [JsonProperty("Final Fantasy XVI")]
        public bool FinalFantasyXVI { get; set; }
        [JsonProperty("Gunfire Reborn")]
        public bool GunfireReborn { get; set; }
        [JsonProperty("Kingdom Hearts Birth by Sleep Final Mix")]
        public bool KingdomHeartsBirthBySleepFinalMix { get; set; }
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
        [JsonProperty("Resident Evil")]
        public bool ResidentEvil { get; set; }
        [JsonProperty("Resident Evil 4 (2005)")]
        public bool ResidentEvil42005 { get; set; }
        [JsonProperty("Resident Evil 4 Remake")]
        public bool ResidentEvil4Remake { get; set; }
        [JsonProperty("Resident Evil 5")]
        public bool ResidentEvil5 { get; set; }
        [JsonProperty("Resident Evil 6")]
        public bool ResidentEvil6 { get; set; }
        [JsonProperty("Resident Evil Revelations 2")]
        public bool ResidentEvilRevelations2 { get; set; }
        [JsonProperty("Sonic Adventure 2")]
        public bool SonicAdventure2 { get; set; }
        [JsonProperty("Streets of Rage Remake")]
        public bool StreetsofRageRemake { get; set; }
        [JsonProperty("Temtem: Swarm")]
        public bool TemtemSwarm { get; set; }
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
                case "CRISIS CORE –FINAL FANTASY VII– REUNION":
                    return CrisisCoreFinalFantasyVIIReunion;
                case "Final Fantasy VII Remake":
                    return FinalFantasyVIIRemake;
                case "Kingdom Hearts Birth by Sleep Final Mix":
                    return KingdomHeartsBirthBySleepFinalMix;
                case "Kingdom Hearts Dream Drop Distance":
                    return KingdomHeartsDreamDropDistance;
                case "Kingdom Hearts Final Mix":
                    return KingdomHeartsFinalMix;
                case "Kingdom Hearts II Final Mix":
                    return KingdomHeartsIIFinalMix;
                case "Kingdom Hearts III":
                    return KingdomHeartsIII;
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
                case "Resident Evil":
                    return ResidentEvil;
                case "Resident Evil 4 (2005)":
                    return ResidentEvil42005;
                case "Resident Evil 5":
                    return ResidentEvil5;
                case "Resident Evil 6":
                    return ResidentEvil6;
                case "Resident Evil Revelations 2":
                    return ResidentEvilRevelations2;
                case "Sonic Adventure 2":
                    return SonicAdventure2;
                case "Streets of Rage Remake":
                    return StreetsofRageRemake;
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

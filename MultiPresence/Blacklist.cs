using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPresence
{
    public class Blacklist
    {
        [JsonProperty("Kingdom Hearts Birth by Sleep Final Mix")]
        public bool KingdomHeartsBirthBySleepFinalMix { get; set; }
        [JsonProperty("Kingdom Hearts Dream Drop Distance")]
        public bool KingdomHeartsDreamDropDistance { get; set; }
        [JsonProperty("Kingdom Hearts Final Mix")]
        public bool KingdomHeartsFinalMix { get; set; }
        [JsonProperty("Kingdom Hearts II Final Mix")]
        public bool KingdomHeartsIIFinalMix { get; set; }
        [JsonProperty("Kingdom Hearts III Final Mix")]
        public bool KingdomHeartsIIIFinalMix { get; set; }
        [JsonProperty("Mega Man 11")]
        public bool MegaMan11 { get; set; }
        [JsonProperty("Mega Man Battle Network 6: Cybeast Gregar")]
        public bool MegaManBattleNetwork6CybeastGregar { get; set; }
        [JsonProperty("Mega Man X: Command Mission")]
        public bool MegaManXCommandMission { get; set; }
        [JsonProperty("Pangya Reborn")]
        public bool PangyaReborn { get; set; }
        [JsonProperty("Ratchet: Deadlocked")]
        public bool RatchetDeadlocked { get; set; }
        [JsonProperty("Resident Evil")]
        public bool ResidentEvil { get; set; }
        [JsonProperty("Resident Evil 4 (2005)")]
        public bool ResidentEvil42005 { get; set; }
        [JsonProperty("Resident Evil 5")]
        public bool ResidentEvil5 { get; set; }
        [JsonProperty("Resident Evil 6")]
        public bool ResidentEvil6 { get; set; }
        [JsonProperty("Resident Evil Revelations 2")]
        public bool ResidentEvilRevelations2 { get; set; }
        [JsonProperty("Sonic Adventure 2")]
        public bool SonicAdventure2 { get; set; }
        [JsonProperty("TY the Tasmanian Tiger")]
        public bool TYTheTasmanianTiger { get; set; }
        [JsonProperty("Zelda: The Wind Waker HD")]
        public bool ZeldaTheWindWakerHD { get; set; }
        [JsonProperty("Zelda: Twilight Princess HD")]
        public bool ZeldaTwilightPrincessHD { get; set; }

        public bool GetValue(string gameTitle)
        {
            switch (gameTitle)
            {
                case "Kingdom Hearts Birth by Sleep Final Mix":
                    return KingdomHeartsBirthBySleepFinalMix;
                case "Kingdom Hearts Dream Drop Distance":
                    return KingdomHeartsDreamDropDistance;
                case "Kingdom Hearts Final Mix":
                    return KingdomHeartsFinalMix;
                case "Kingdom Hearts II Final Mix":
                    return KingdomHeartsIIFinalMix;
                case "Kingdom Hearts III Final Mix":
                    return KingdomHeartsIIIFinalMix;
                case "Mega Man 11":
                    return MegaMan11;
                case "Mega Man Battle Network 6: Cybeast Gregar":
                    return MegaManBattleNetwork6CybeastGregar;
                case "Mega Man X: Command Mission":
                    return MegaManXCommandMission;
                case "Pangya Reborn":
                    return PangyaReborn;
                case "Ratchet: Deadlocked":
                    return RatchetDeadlocked;
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
                case "TY the Tasmanian Tiger":
                    return TYTheTasmanianTiger;
                case "Zelda: The Wind Waker HD":
                    return ZeldaTheWindWakerHD;
                case "Zelda: Twilight Princess HD":
                    return ZeldaTwilightPrincessHD;
                default:
                    return false;
            }
        }
    }
}

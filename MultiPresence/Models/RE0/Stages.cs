namespace MultiPresence.Models.RE0
{
    public class Stages
    {
        public static async Task<string> GetStage(int stageid)
        {
            string stage;

            switch (stageid)
            {
                case 0:
                    stage = "Train Cockpit";
                    break;
                case 1:
                    stage = "Train Platform (Front)";
                    break;
                case 2:
                case 16:
                case 18:
                case 19:
                case 27:
                    stage = "Officer's Car";
                    break;
                case 4:
                    stage = "Cabin 201";
                    break;
                case 5:
                case 29:
                    stage = "Cabin 202";
                    break;
                case 6:
                case 30:
                    stage = "Passenger Car 2";
                    break;
                case 7:
                case 31:
                    stage = "Passenger Car 1";
                    break;
                case 8:
                case 21:
                case 34:
                case 38:
                    stage = "Dining Car";
                    break;
                case 9:
                    stage = "Cargo";
                    break;
                case 10:
                    stage = "Train Platfrom (Back)";
                    break;
                case 11:
                    stage = "Scorpion Hall";
                    break;
                case 12:
                case 23:
                case 24:
                    stage = "2F Leech Hall";
                    break;
                case 13:
                    stage = "2F Passenger Cabin A";
                    break;
                case 14:
                    stage = "2F Passenger Cabin B";
                    break;
                case 15:
                case 25:
                case 33:
                    stage = "Restaurant Car";
                    break;
                case 17:
                    stage = "Service Room";
                    break;
                case 22:
                case 35:
                    stage = "2F Scorpion Room";
                    break;
                case 28:
                    stage = "1F Save Room";
                    break;
                case 32:
                    stage = "2F Hallway";
                    break;
                case 36:
                case 46:
                case 77:
                    stage = "Foyer";
                    break;
                case 37:
                    stage = "1F Dining Room";
                    break;
                case 39:
                    stage = "1F Projector Room";
                    break;
                case 40:
                    stage = "1F West Hall";
                    break;
                case 41:
                    stage = "1F Washroom";
                    break;
                case 42:
                    stage = "Basement";
                    break;
                case 43:
                case 68:
                    stage = "3F Early Control Room";
                    break;
                case 44:
                    stage = "1F Early Storage";
                    break;
                case 45:
                    stage = "Main Entrance";
                    break;
                case 47:
                case 69:
                case 70:
                case 71:
                case 72:
                    stage = "2F Auditorium";
                    break;
                case 48:
                    stage = "Traning Facility Hall";
                    break;
                case 50:
                    stage = "Library";
                    break;
                case 51:
                case 73:
                    stage = "2F Moose Head Room";
                    break;
                case 52:
                    stage = "Library Back Room";
                    break;
                case 53:
                    stage = "2F Hall";
                    break;
                case 54:
                    stage = "Chessboard Room";
                    break;
                case 55:
                case 74:
                    stage = "2F Lounge";
                    break;
                case 56:
                    stage = "2F Dining Room";
                    break;
                case 57:
                    stage = "2F Gallery";
                    break;
                case 58:
                    stage = "2F Hallway A";
                    break;
                case 59:
                case 64:
                case 75:
                    stage = "Early 2F Hallway";
                    break;
                case 60:
                    stage = "2F Testing Room";
                    break;
                case 61:
                    stage = "2F Workshop";
                    break;
                case 62:
                    stage = "2F Bar";
                    break;
                case 63:
                    stage = "2F Monitor Room";
                    break;
                case 65:
                case 78:
                case 79:
                case 83:
                    stage = "Centipede Domain";
                    break;
                case 66:
                case 76:
                    stage = "BF Train Crash Site";
                    break;
                case 67:
                    stage = "BF Sewer";
                    break;
                case 80:
                case 81:
                case 82:
                case 100:
                case 115:
                case 116:
                case 122:
                case 123:
                case 124:
                case 125:
                case 126:
                case 127:
                case 128:
                case 129:
                case 130:
                    stage = "Chapel Altar";
                    break;
                case 86:
                    stage = "BF Hallway";
                    break;
                case 87:
                    stage = "BF Lodgings A";
                    break;
                case 88:
                case 98:
                    stage = "BF Torture Chamber/Power Room";
                    break;
                case 89:
                    stage = "BF Study/Save Room";
                    break;
                case 90:
                    stage = "Early BF Main Hall";
                    break;
                case 91:
                    stage = "Basement (Hole)";
                    break;
                case 92:
                    stage = "BF Lodgings B";
                    break;
                case 93:
                    stage = "BF Lodgings C";
                    break;
                case 94:
                    stage = "Early BF2 Storage";
                    break;
                case 95:
                    stage = "BF2 Hallway";
                    break;
                case 96:
                    stage = "BF2 Battle Arena";
                    break;
                case 97:
                    stage = "BF Prison";
                    break;
                case 99:
                    stage = "Early Bridge";
                    break;
                case 101:
                    stage = "Chapel Roof";
                    break;
                case 102:
                    stage = "Gondola Platform (Lower)";
                    break;
                case 103:
                    stage = "Main Hall (Lower)";
                    break;
                case 104:
                    stage = "Gondola Platform (Upper)";
                    break;
                case 105:
                    stage = "2F Experimental Room A";
                    break;
                case 106:
                    stage = "Early Gallery";
                    break;
                case 107:
                    stage = "2F Experimental Room B";
                    break;
                case 108:
                    stage = "2F Morgue";
                    break;
                case 109:
                    stage = "2F Specimen Room";
                    break;
                case 110:
                    stage = "Unknown Early Lab";
                    break;
                case 111:
                    stage = "Gondola Interior";
                    break;
                case 112:
                    stage = "Early Library";
                    break;
                case 113:
                    stage = "Marcus' Study";
                    break;
                case 114:
                    stage = "Chapel Save Room";
                    break;
                case 117:
                case 118:
                case 119:
                case 120:
                case 132:
                    stage = "Gondola Platform";
                    break;
                case 121:
                    stage = "Early Upper Hallway";
                    break;
                case 133:
                    stage = "Walkway";
                    break;
                case 134:
                case 137:
                case 138:
                    stage = "Lift Top Floor";
                    break;
                case 135:
                    stage = "Monitor Room";
                    break;
                case 136:
                    stage = "Lift Control/Save Room";
                    break;
                case 139:
                    stage = "Tyrant Battleground";
                    break;
                case 140:
                    stage = "Factory Main Hall";
                    break;
                case 141:
                    stage = "Bridge";
                    break;
                case 142:
                    stage = "Power Control Room";
                    break;
                case 143:
                    stage = "Pipe Room Balcony";
                    break;
                case 144:
                    stage = "Pipe Room";
                    break;
                case 145:
                    stage = "Early Hallway";
                    break;
                case 146:
                    stage = "Lounge";
                    break;
                case 147:
                    stage = "Unknown Early Hallway";
                    break;
                case 148:
                    stage = "Lodgings";
                    break;
                case 149:
                case 150:
                    stage = "Pool Room";
                    break;
                case 151:
                    stage = "Upper Bridge";
                    break;
                case 152:
                    stage = "Elevtor/Gondola Room (Upper)";
                    break;
                case 153:
                    stage = "Forklift Area";
                    break;
                case 154:
                    stage = "Crate Puzzle Room";
                    break;
                case 155:
                    stage = "Stairwell";
                    break;
                case 156:
                    stage = "Sewage Dump";
                    break;
                case 157:
                    stage = "Specimen Room (Upper)";
                    break;
                case 158:
                case 167:
                case 168:
                case 169:
                case 170:
                    stage = "Dam Control Room";
                    break;
                case 159:
                    stage = "Balcony";
                    break;
                case 160:
                    stage = "Elevator Room (Lower)";
                    break;
                case 161:
                case 171:
                    stage = "Specimen Room (Lower)";
                    break;
                case 162:
                    stage = "Turbine Hallway";
                    break;
                case 163:
                    stage = "Dam";
                    break;
                case 164:
                    stage = "Leech Queen Lair";
                    break;
                case 165:
                    stage = "Helipad";
                    break;
                case 166:
                    stage = "Main Elevator";
                    break;

                default:
                    stage = "Unknown stage";
                    break;
            }

            return stage;
        }
    }
}

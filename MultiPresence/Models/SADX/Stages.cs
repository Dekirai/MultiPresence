namespace MultiPresence.Models.SADX
{
    public class Stages
    {
        public static async Task<string> GetStage(int stageid)
        {
            string stage;

            switch (stageid)
            {
                case 0:
                    stage = "Hedgehog Hammer";
                    break;
                case 1:
                    stage = "Emerald Coast";
                    break;
                case 2:
                    stage = "Windy Valley";
                    break;
                case 3:
                    stage = "Twinkle Park";
                    break;
                case 4:
                    stage = "Speed Highway";
                    break;
                case 5:
                    stage = "Red Mountain";
                    break;
                case 6:
                    stage = "Sky Deck";
                    break;
                case 7:
                    stage = "Lost World";
                    break;
                case 8:
                    stage = "Ice Cap";
                    break;
                case 9:
                    stage = "Casinopolis";
                    break;
                case 10:
                    stage = "Final Egg";
                    break;
                case 12:
                    stage = "Hot Shelter";
                    break;
                case 15:
                    stage = "Chaos 0";
                    break;
                case 16:
                    stage = "Chaos 2";
                    break;
                case 17:
                    stage = "Chaos 4";
                    break;
                case 18:
                    stage = "Chaos 6";
                    break;
                case 19:
                    stage = "Perfect Chaos";
                    break;
                case 20:
                    stage = "Egg Hornet";
                    break;
                case 21:
                    stage = "Egg Walker";
                    break;
                case 22:
                    stage = "Egg Viper";
                    break;
                case 23:
                    stage = "ZERO";
                    break;
                case 24:
                    stage = "E-101";
                    break;
                case 25:
                    stage = "E-101mkII";
                    break;
                case 26:
                    stage = "Station Square";
                    break;
                case 29:
                case 32:
                    stage = "Egg Carrier";
                    break;
                case 33:
                    stage = "Mystic Ruins";
                    break;
                case 34:
                    stage = "Mystic Ruins (Past)";
                    break;
                case 35:
                    stage = "Twinkle Circuit";
                    break;
                case 36:
                    stage = "Sky Chase Act 1";
                    break;
                case 37:
                    stage = "Sky Chase Act 2";
                    break;
                case 38:
                    stage = "Sand Hill";
                    break;
                case 39:
                    stage = "Station Square - Chao Garden";
                    break;
                case 40:
                    stage = "Mystic Ruins - Chao Garden";
                    break;
                case 41:
                    stage = "Egg Carrier - Chao Garden";
                    break;
                case 42:
                    stage = "Chao Race";
                    break;

                default:
                    stage = "Unknown stage";
                    break;
            }

            return stage;
        }
    }
}
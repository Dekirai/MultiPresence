namespace MultiPresence.Models.SA2
{
    public class Stages
    {
        public static async Task<string> GetStage(int stageid)
        {
            string stage;

            switch (stageid)
            {
                case 0:
                    stage = "Basic Test";
                    break;
                case 1:
                    stage = "Knuckles Test";
                    break;
                case 2:
                    stage = "Sonic Test";
                    break;
                case 3:
                    stage = "Green Forest";
                    break;
                case 4:
                    stage = "White Jungle";
                    break;
                case 5:
                    stage = "Pumpkin Hill";
                    break;
                case 6:
                    stage = "Sky Rail";
                    break;
                case 7:
                    stage = "Aquatic Mine";
                    break;
                case 8:
                    stage = "Security Hall";
                    break;
                case 9:
                    stage = "Prison Lane";
                    break;
                case 10:
                case 57:
                    stage = "Metal Harbor";
                    break;
                case 11:
                case 58:
                    stage = "Iron Gate";
                    break;
                case 12:
                case 15:
                    stage = "Weapons Bed";
                    break;
                case 13:
                    stage = "City Escape";
                    break;
                case 14:
                    stage = "Radical Highway";
                    break;
                case 16:
                case 41:
                    stage = "Wild Canyon";
                    break;
                case 17:
                case 39:
                    stage = "Mission Street";
                    break;
                case 18:
                case 46:
                    stage = "Dry Lagoon";
                    break;
                case 19:
                    stage = "Sonic VS. Shadow 1";
                    break;
                case 20:
                    stage = "Tails VS. Eggman 1";
                    break;
                case 21:
                case 45:
                    stage = "Sand Ocean";
                    break;
                case 22:
                    stage = "Crazy Gadget";
                    break;
                case 23:
                case 48:
                    stage = "Hidden Base";
                    break;
                case 24:
                case 56:
                    stage = "Eternal Engine";
                    break;
                case 25:
                case 59:
                    stage = "Death Chamber";
                    break;
                case 26:
                    stage = "Egg Quarters";
                    break;
                case 27:
                case 55:
                    stage = "Lost Colony";
                    break;
                case 28:
                    stage = "Pyramid Cave";
                    break;
                case 29:
                    stage = "Tails VS. Eggman 2";
                    break;
                case 30:
                    stage = "Final Rush";
                    break;
                case 31:
                    stage = "Green Hill";
                    break;
                case 32:
                    stage = "Meteor Head";
                    break;
                case 33:
                    stage = "Knuckles VS. Rouge";
                    break;
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                    stage = "Cannon's Core";
                    break;
                case 40:
                    stage = "Final Chase";
                    break;
                case 42:
                    stage = "Sonic VS. Shadow 2";
                    break;
                case 43:
                case 53:
                    stage = "Cosmic Wall";
                    break;
                case 44:
                    stage = "Mad Space";
                    break;
                case 47:
                    stage = "Pyramid Race";
                    break;
                case 49:
                    stage = "Pool Quest";
                    break;
                case 50:
                    stage = "Planet Quest";
                    break;
                case 51:
                    stage = "Deck Race";
                    break;
                case 52:
                    stage = "Downtown Race";
                    break;
                case 54:
                    stage = "Grind Race";
                    break;
                case 60:
                    stage = "F-6t BIG FOOT";
                    break;
                case 61:
                    stage = "B-3x HOT SHOT";
                    break;
                case 62:
                    stage = "R-1/A FLYING DOG";
                    break;
                case 63:
                    stage = "King Boom Boo";
                    break;
                case 64:
                case 67:
                    stage = "Egg Golem";
                    break;
                case 65:
                    stage = "Biolizard";
                    break;
                case 66:
                    stage = "Final Hazard";
                    break;
                case 70:
                    stage = "Route 101/Route 280";
                    break;
                case 71:
                    stage = "Kart Race";
                    break;
                case 90:
                    stage = "Chao World";
                    break;

                default:
                    stage = "Unknown stage";
                    break;
            }

            return stage;
        }
    }
}
namespace MultiPresence.Models.RE5
{
    public class Stages
    {
        public static async Task<string> GetStage(int stageid)
        {
            string stage;

            switch (stageid)
            {
                case 100:
                    stage = "Chapter 1-1:Civilian Checkpoint";
                    break;
                case 115:
                    stage = "Chapter 1-1:Back Alley";
                    break;
                case 116:
                    stage = "Chapter 1-1:Back Alley";
                    break;
                case 114:
                    stage = "Chapter 1-2:Public Assembly";
                    break;
                case 102:
                    stage = "Chapter 1-2:Public Urban District";
                    break;
                case 117:
                    stage = "Chapter 1-2:Abandoned Building";
                    break;
                case 103:
                    stage = "Chapter 1-2:Furnace Facility";
                    break;
                case 118:
                    stage = "Chapter 2-1:Storage Facility";
                    break;
                case 104:
                    stage = "Chapter 2-1:The Bridge";
                    break;
                case 113:
                    stage = "Chapter 2-1:The Port";
                    break;
                case 105:
                    stage = "Chapter 2-1:Shanty Town";
                    break;
                case 106:
                    stage = "Chapter 2-1:Train Yard";
                    break;
                case 119:
                    stage = "Chapter 2-2:Train Station";
                    break;
                case 107:
                    stage = "Chapter 2-2:The Mines";
                    break;
                case 108:
                    stage = "Chapter 2-2:Mining Area";
                    break;
                case 109:
                    stage = "Chapter 2-3:Savanna";
                    break;
                case 111:
                    stage = "Chapter 2-3:The Port";
                    break;
                case 200:
                    stage = "Chapter 3-1:Marshlands";
                    break;
                case 202:
                    stage = "Chapter 3-1:Village";
                    break;
                case 201:
                    stage = "Chapter 3-2:Execution Ground";
                    break;
                case 207:
                    stage = "Chapter 3-2:Refinery";
                    break;
                case 203:
                    stage = "Chapter 3-2:Control Facility";
                    break;
                case 209:
                    stage = "Chapter 3-2:Dock";
                    break;
                case 204:
                    stage = "Chapter 3-3:Drilling Facilities";
                    break;
                case 205:
                    stage = "Chapter 3-3:Patrol Boat";
                    break;
                case 313:
                    stage = "Chapter 4-1:Caves";
                    break;
                case 300:
                    stage = "Chapter 4-1:Acient Village";
                    break;
                case 301:
                    stage = "Chapter 4-1:Labyrinth";
                    break;
                case 302:
                    stage = "Chapter 4-2:Worship Area";
                    break;
                case 303:
                    stage = "Chapter 4-2:Pyramid";
                    break;
                case 312:
                    stage = "Chapter 4-2:Underground Garden";
                    break;
                case 304:
                    stage = "Chapter 5-1:Experimental Facility";
                    break;
                case 310:
                    stage = "Chapter 5-2:Power Station";
                    break;
                case 307:
                    stage = "Chapter 5-2:Missile Area 1st Floor";
                    break;
                case 316:
                    stage = "Chapter 5-2:Experimental Facility Passage";
                    break;
                case 308:
                    stage = "Chapter 5-2:Uroboros Research Facility";
                    break;
                case 314:
                    stage = "Chapter 5-3:Missile Area 2nd Floor";
                    break;
                case 315:
                    stage = "Chapter 5-3:Moving Platform";
                    break;
                case 311:
                    stage = "Chapter 5-3:Monarch Room Entrance";
                    break;
                case 309:
                    stage = "Chapter 5-3:Monarch Room";
                    break;
                case 500:
                    stage = "Chapter 6-1:Ship Deck";
                    break;
                case 501:
                    stage = "Chapter 6-1:Ship Hold";
                    break;
                case 503:
                    stage = "Chapter 6-2:Main Deck";
                    break;
                case 504:
                    stage = "Chapter 6-2:Bridge";
                    break;
                case 511:
                    stage = "Chapter 6-3:Bridge Deck";
                    break;
                case 512:
                    stage = "Chapter 6-3:Bridge";
                    break;
                case 505:
                    stage = "Chapter 6-3:Engine Room";
                    break;
                case 506:
                    stage = "Chapter 6-3:Hangar";
                    break;
                case 508:
                    stage = "Chapter 6-3:Volcano";
                    break;
                case 400:
                    stage = "Mercenaries:Public Assembly";
                    break;
                case 401:
                    stage = "Mercenaries:The Mines";
                    break;
                case 402:
                    stage = "Mercenaries:Village";
                    break;
                case 403:
                    stage = "Mercenaries:Ancient Ruins";
                    break;
                case 404:
                    stage = "Mercenaries:Experimental Facility";
                    break;
                case 405:
                    stage = "Mercenaries:Missile Area";
                    break;
                case 406:
                    stage = "Mercenaries:Ship Deck";
                    break;
                case 407:
                    stage = "Mercenaries:Prison";
                    break;
                case 700:
                    stage = "Lost in Nightmares:Entrance";
                    break;
                case 701:
                    stage = "Lost in Nightmares:Prison";
                    break;
                case 702:
                    stage = "Desparate Escape:First Action";
                    break;
                case 703:
                    stage = "Desparate Escape:Second Action";
                    break;
                case 704:
                    stage = "Desparate Escape:Hangar Final Action";
                    break;
                case 705:
                    stage = "Lost in Nightmares:After Prison";
                    break;
                case 706:
                    stage = "Lost in Nightmares:Wesker Fight";
                    break;

                default:
                    stage = "Unknown stage";
                    break;
            }

            return stage;
        }
    }
}
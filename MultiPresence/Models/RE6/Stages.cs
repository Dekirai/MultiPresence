namespace MultiPresence.Models.RE6
{
    public class Stages
    {
        public static async Task<string> GetStage(int stageid)
        {
            string stage;

            switch (stageid)
            {
                case 0x0303:
                    stage = "Prelude";
                    break;
                case 0x0068:
                    stage = "Chapter 1-1 (Leon):Campus - Visitors Room";
                    break;
                case 0x0069:
                    stage = "Chapter 1-2 (Leon):Campus";
                    break;
                case 0x0065:
                    stage = "Chapter 1-3 (Leon):Underground";
                    break;
                case 0x0066:
                    stage = "Chapter 1-4 (Leon):The Town";
                    break;
                case 0x0067:
                    stage = "Chapter 1-5 (Leon):Gun Shop";
                    break;
                case 0x00D2:
                    stage = "Chapter 2-1 (Leon):The Bus";
                    break;
                case 0x00C8:
                    stage = "Chapter 2-2 (Leon):Graveyard";
                    break;
                case 0x00C9:
                    stage = "Chapter 2-3 (Leon):Cathedral";
                    break;
                case 0x00CA:
                    stage = "Chapter 2-4 (Leon):Underground Lab";
                    break;
                case 0x0117:
                    stage = "Chapter 2-5 (Leon):Primitive Altar Entrance";
                    break;
                case 0x00D0:
                    stage = "Chapter 2-6 (Leon):Primitive Altar";
                    break;
                case 0x00CB:
                    stage = "Chapter 3-1 (Leon):Altar Corridor";
                    break;
                case 0x00CC:
                    stage = "Chapter 3-2 (Leon):Catacombs";
                    break;
                case 0x00CE:
                    stage = "Chapter 3-3 (Leon):Cavern";
                    break;
                case 0x00FA:
                    stage = "Chapter 3-4 (Leon):Underground Water Channel";
                    break;
                case 0x0228:
                    stage = "Chapter 4-1 (Leon):Inside the Airplane";
                    break;
                case 0x01FE:
                    stage = "Chapter 4-2 (Leon):Airplane Crash Site";
                    break;
                case 0x01FF:
                    stage = "Chapter 4-3 (Leon):Market";
                    break;
                case 0x0200:
                    stage = "Chapter 4-4 (Leon):Medical Research Center";
                    break;
                case 0x0202:
                    stage = "Chapter 4-5 (Leon):Train";
                    break;
                case 0x0302:
                    stage = "Chapter 5-1 (Leon):Port Area";
                    break;
                case 0x02BD:
                    stage = "Chapter 5-2 (Leon):High-Rise Area";
                    break;
                case 0x02C2:
                    stage = "Chapter 5-3 (Leon):Quad Tower Entrance";
                    break;
                case 0x02BE:
                    stage = "Chapter 5-4 (Leon):Quad Tower";
                    break;
                case 0x0305:
                    stage = "Chapter 5-5 (Leon):Quad Tower Roof";
                    break;
                case 0x02BC:
                    stage = "Leon:Final Level";
                    break;
                case 0x0130:
                    stage = "Chapter 1-1 (Jake):The Sewer";
                    break;
                case 0x0131:
                    stage = "Chapter 1-2 (Jake):Water Channel";
                    break;
                case 0x0133:
                    stage = "Chapter 1-3 (Jake):Underground Passage";
                    break;
                //case 0x0000:
                //    stage = "Chapter 1-4 (Jake):In Front of City Hall";
                //    break;
                case 0x0132:
                    stage = "Chapter 1-5 (Jake):Inside the Helicopter";
                    break;
                case 0x0190:
                    stage = "Chapter 2-1 (Jake):Mountain Path";
                    break;
                case 0x0191:
                    stage = "Chapter 2-2 (Jake):Snow-Covered Mountain";
                    break;
                case 0x0192:
                    stage = "Chapter 2-3 (Jake):Cave";
                    break;
                case 0x0258:
                    stage = "Chapter 3-1 (Jake):Research Facility - Detention Center";
                    break;
                case 0x0259:
                    stage = "Chapter 3-2 (Jake):Research Facility - Living Quarters";
                    break;
                case 0x025A:
                    stage = "Chapter 3-3 (Jake):Research Facility - Entrance";
                    break;
                case 0x0227:
                    stage = "Chapter 4-1 (Jake):City and Highway";
                    break;
                //case 0x0000:
                //    stage = "Chapter 4-2 (Jake):Poisawan Courtyard";
                //    break;
                case 0x0203:
                    stage = "Chapter 4-3 (Jake):Poisawan - Shopping District";
                    break;
                case 0x0243:
                    stage = "Chapter 4-4 (Jake):Shopping District";
                    break;
                //case 0x0000:
                //    stage = "Chapter 4-5 (Jake):Airplane Crash Site";
                //    break;
                case 0x0242:
                    stage = "Chapter 4-6 (Jake):Stilt Housing Area";
                    break;
                case 0x0388:
                    stage = "Chapter 5-1 (Jake):Underwater Facility";
                    break;
                //case 0x0000:
                //    stage = "Chapter 5-2 (Jake):Underwater Facility - Lower Levels";
                //    break;
                case 0x0389:
                    stage = "Chapter 5-3 (Jake):Underwater Facility";
                    break;
                case 0x03B6:
                    stage = "Chapter 5-4 (Jake):Shipping Center";
                    break;
                case 0x01F4:
                    stage = "Chapter 1-1 (Chris):Main Street";
                    break;
                case 0x01F5:
                    stage = "Chapter 1-2 (Chris):Back Street";
                    break;
                case 0x01F6:
                    stage = "Chapter 1-3 (Chris):Rooftops";
                    break;
                case 0x01F7:
                    stage = "Chapter 1-4 (Chris):Tenement";
                    break;
                case 0x012C:
                    stage = "Chapter 2-1 (Chris):City in Eastern Europe";
                    break;
                case 0x012D:
                    stage = "Chapter 2-2 (Chris):The Bridge";
                    break;
                case 0x012E:
                    stage = "Chapter 2-3 (Chris):In Front of City Hall";
                    break;
                case 0x012F:
                    stage = "Chapter 2-4 (Chris):City Hall";
                    break;
                case 0x01F8:
                    stage = "Chapter 3-1 (Chris):Tenement - Poisawan Entrance";
                    break;
                case 0x01FA:
                    stage = "Chapter 3-2 (Chris):Poisawan Courtyard";
                    break;
                case 0x01FB:
                    stage = "Chapter 3-3 (Chris):Poisawan Inner Area";
                    break;
                case 0x01FC:
                    stage = "Chapter 3-4 (Chris):Stilt Housing Area";
                    break;
                //case 0x0000:
                //    stage = "Chapter 3-5 (Chris):Medical Research Center";
                //    break;
                case 0x0226:
                    stage = "Chapter 3-6 (Chris):Main Throughfare";
                    break;
                case 0x0320:
                    stage = "Chapter 4-1 (Chris):Aircraft Carrier - Rear Hangar";
                    break;
                case 0x0321:
                    stage = "Chapter 4-2 (Chris):Aircraft Carrier - Bridge";
                    break;
                case 0x0368:
                    stage = "Chapter 4-3 (Chris):Aircraft Carrier - Forward Hangar";
                    break;
                case 0x0353:
                    stage = "Chapter 4-4 (Chris):Airspace over Aircraft Carrier";
                    break;
                case 0x0385:
                    stage = "Chapter 5-1 (Chris):Underwater Facility";
                    break;
                case 0x0386:
                    stage = "Chapter 5-2 (Chris):Underwater Facility - Lower Levels";
                    break;
                //case 0x0000:
                //    stage = "Chapter 5-3 (Chris):Underwater Facility - Upper Levels";
                //    break;
                case 0x0387:
                    stage = "Chapter 5-4 (Chris):Emergency Escape Route";
                    break;
                case 0x03CD:
                    stage = "Chapter 5-5 (Chris):Emergency Escape Route";
                    break;
                case 0x03CC:
                    stage = "Chapter 5-0 (Chris):Destruction of the Underwater Facility";
                    break;
                case 0x03E8:
                    stage = "Chapter 1-1 (Ada):Submarine Interior";
                    break;
                case 0x03E9:
                    stage = "Chapter 1-2 (Ada):Submarine - Reactor";
                    break;
                case 0x03EB:
                    stage = "Chapter 1-3 (Ada):Submarine - Torpedo Room";
                    break;
                case 0x00CF:
                    stage = "Chapter 2-1 (Ada):Forest Cemetery";
                    break;
                //case 0x0000:
                //    stage = "Chapter 2-2 (Ada):Altar Corridor";
                //    break;
                case 0x0110:
                    stage = "Chapter 2-3 (Ada):Underground Lab";
                    break;
                case 0x023E:
                    stage = "Chapter 3-1 (Ada):Tenement - Bin Street";
                    break;
                case 0x01FD:
                    stage = "Chapter 3-2 (Ada):Shopping District";
                    break;
                case 0x0204:
                    stage = "Chapter 3-3 (Ada):Train";
                    break;
                //case 0x0000:
                //    stage = "Chapter 3-4 (Ada):Stilt Housing Area";
                //    break;
                case 0x0322:
                    stage = "Chapter 4-1 (Ada):Aircraft Carrier - Forward Hangar";
                    break;
                case 0x0367:
                    stage = "Chapter 4-2 (Ada):Aircraft Carrier - Bridge";
                    break;
                case 0x0324:
                    stage = "Chapter 4-3 (Ada):Aircraft Carrier Interior";
                    break;
                case 0x02EF:
                    stage = "Chapter 5-1 (Ada):High-Rise Area";
                    break;
                //case 0x0000:
                //    stage = "Chapter 5-2 (Ada):Quad Tower Entrance";
                //    break;
                case 0x02BF:
                    stage = "Chapter 5-3 (Ada):Quad Tower Roof";
                    break;
                //case 0x0000:
                //    stage = "Chapter 5-4 (Ada):Quad Tower";
                //    break;
                case 0x02C1:
                    stage = "Chapter 5 (Ada):Final Level";
                    break;
                case 0x04B0:
                    stage = "Mercenaries:Rail Yard";
                    break;
                case 0x04B1:
                    stage = "Mercenaries:Requiem for War";
                    break;
                case 0x04B2:
                    stage = "Mercenaries:Urban Chaos";
                    break;
                case 0x04B3:
                    stage = "Mercenaries:Mining the Depths";
                    break;
                case 0x04B4:
                    stage = "Mercenaries:High Seas Fortress";
                    break;
                case 0x04B5:
                    stage = "Mercenaries:Catacombs";
                    break;
                case 0x04B6:
                    stage = "Mercenaries:Steel Beast";
                    break;
                case 0x04B7:
                    stage = "Mercenaries:Rooftop";
                    break;
                case 0x04B8:
                    stage = "Mercenaries:Creature Workshop";
                    break;
                case 0x04B9:
                    stage = "Mercenaries:Liquid Fire";
                    break;
                case 0x04BA:
                    stage = "No Mercy:Rail Yard";
                    break;
                case 0x04BB:
                    stage = "No Mercy:Requiem for War";
                    break;
                case 0x04BC:
                    stage = "No Mercy:Urban Chaos";
                    break;
                case 0x04BD:
                    stage = "No Mercy:Mining the Depths";
                    break;
                case 0x04BE:
                    stage = "No Mercy:High Seas Fortress";
                    break;
                case 0x04BF:
                    stage = "No Mercy:Catacombs";
                    break;
                case 0x04C0:
                    stage = "No Mercy:Steel Beast";
                    break;
                case 0x04C1:
                    stage = "No Mercy:Rooftop";
                    break;
                case 0x04C2:
                    stage = "No Mercy:Creature Workshop";
                    break;
                case 0x04C3:
                    stage = "No Mercy:Liquid Fire";
                    break;
                case 0x0514:
                    stage = "Onslaught:Rail Yard";
                    break;
                case 0x0515:
                    stage = "Onslaught:Requiem for War";
                    break;
                case 0x0516:
                    stage = "Onslaught:Urban Chaos";
                    break;
                case 0x0517:
                    stage = "Onslaught:Mining the Depths";
                    break;
                case 0x0518:
                    stage = "Onslaught:High Seas Fortress";
                    break;
                case 0x0519:
                    stage = "Onslaught:Catacombs";
                    break;
                case 0x051A:
                    stage = "Onslaught:Steel Beast";
                    break;
                case 0x051B:
                    stage = "Onslaught:Rooftop";
                    break;
                case 0x051C:
                    stage = "Onslaught:Creature Workshop";
                    break;
                case 0x051D:
                    stage = "Onslaught:Liquid Fire";
                    break;
                case 0x0578:
                    stage = "Survivors:Rail Yard";
                    break;
                case 0x0579:
                    stage = "Survivors:Requiem for War";
                    break;
                case 0x057A:
                    stage = "Survivors:Urban Chaos";
                    break;
                case 0x057B:
                    stage = "Survivors:Mining the Depths";
                    break;
                case 0x057C:
                    stage = "Survivors:High Seas Fortress";
                    break;
                case 0x057D:
                    stage = "Survivors:Catacombs";
                    break;
                case 0x057E:
                    stage = "Survivors:Steel Beast";
                    break;
                case 0x057F:
                    stage = "Survivors:Rooftop";
                    break;
                case 0x0580:
                    stage = "Survivors:Creature Workshop";
                    break;
                case 0x0581:
                    stage = "Survivors:Liquid Fire";
                    break;
                case 0x05DC:
                    stage = "Predator:Rail Yard";
                    break;
                case 0x05DD:
                    stage = "Predator:Requiem for War";
                    break;
                case 0x05DE:
                    stage = "Predator:Urban Chaos";
                    break;
                case 0x05DF:
                    stage = "Predator:Mining the Depths";
                    break;
                case 0x05E0:
                    stage = "Predator:High Seas Fortress";
                    break;
                case 0x05E1:
                    stage = "Predator:Catacombs";
                    break;
                case 0x05E2:
                    stage = "Predator:Steel Beast";
                    break;
                case 0x05E3:
                    stage = "Predator:Rooftop";
                    break;
                case 0x05E4:
                    stage = "Predator:Creature Workshop";
                    break;
                case 0x05E5:
                    stage = "Predator:Liquid Fire";
                    break;
                case 0x0640:
                    stage = "Siege:Rail Yard";
                    break;
                case 0x0641:
                    stage = "Siege:Requiem for War";
                    break;
                case 0x0642:
                    stage = "Siege:Urban Chaos";
                    break;
                case 0x0643:
                    stage = "Siege:Mining the Depths";
                    break;
                case 0x0644:
                    stage = "Siege:High Seas Fortress";
                    break;
                case 0x0645:
                    stage = "Siege:Catacombs";
                    break;
                case 0x0646:
                    stage = "Siege:Steel Beast";
                    break;
                case 0x0647:
                    stage = "Siege:Rooftop";
                    break;
                case 0x0648:
                    stage = "Siege:Creature Workshop";
                    break;
                case 0x0649:
                    stage = "Siege:Liquid Fire";
                    break;

                default:
                    stage = "Unknown Chapter:Unknown Location";
                    break;
            }

            return stage;
        }
    }
}
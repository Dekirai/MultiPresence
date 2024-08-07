﻿namespace MultiPresence.Models.KH2
{
    public class Worlds
    {
        public static async Task<string[]> GetWorld(int world)
        {
            List<string> getworld = new List<string>();

            switch (world)
            {
                case 1:
                    getworld.Add("Title screen");
                    getworld.Add("logo");
                    break;
                case 2:
                    getworld.Add("Twilight Town");
                    getworld.Add("twilighttown");
                    break;
                case 4:
                    getworld.Add("Hollow Bastion");
                    getworld.Add("hollowbastion");
                    break;
                case 5:
                    getworld.Add("Beast's Castle");
                    getworld.Add("beast");
                    break;
                case 6:
                    getworld.Add("Olympus Coliseum");
                    getworld.Add("olymp");
                    break;
                case 7:
                    getworld.Add("Agrabah");
                    getworld.Add("agrabah");
                    break;
                case 8:
                    getworld.Add("The Land of Dragon's");
                    getworld.Add("landofdragons");
                    break;
                case 9:
                    getworld.Add("100 Acre Wood");
                    getworld.Add("100acre");
                    break;
                case 10:
                    getworld.Add("Pride Lands");
                    getworld.Add("pridelands");
                    break;
                case 11:
                    getworld.Add("Atlantica");
                    getworld.Add("atlantica");
                    break;
                case 12:
                    getworld.Add("Disney Castle");
                    getworld.Add("disney");
                    break;
                case 13:
                    getworld.Add("Timeless River");
                    getworld.Add("timelessriver");
                    break;
                case 14:
                    getworld.Add("Halloween Town");
                    getworld.Add("halloweentown");
                    break;
                case 15:
                    getworld.Add("World Map");
                    getworld.Add("worldmap");
                    break;
                case 16:
                    getworld.Add("Port Royal");
                    getworld.Add("portroyal");
                    break;
                case 17:
                    getworld.Add("Space Paranoids");
                    getworld.Add("spaceparanoids");
                    break;
                case 18:
                    getworld.Add("The World That Never Was");
                    getworld.Add("twtnw");
                    break;
                case 255:
                    getworld.Add("Main Menu");
                    getworld.Add("logo");
                    break;
                default:
                    getworld.Add("Undefined location");
                    getworld.Add("logo");
                    break;
            }
            return getworld.ToArray();
        }

        public static async Task<string[]> GetWorldDE(int world)
        {
            List<string> getworld = new List<string>();

            switch (world)
            {
                case 1:
                    getworld.Add("Titelbild");
                    getworld.Add("logo");
                    break;
                case 2:
                    getworld.Add("Twilight Town");
                    getworld.Add("twilighttown");
                    break;
                case 4:
                    getworld.Add("Hollow Bastion");
                    getworld.Add("hollowbastion");
                    break;
                case 5:
                    getworld.Add("Das Schloss des Biestes");
                    getworld.Add("beast");
                    break;
                case 6:
                    getworld.Add("Arena des Olymps");
                    getworld.Add("olymp");
                    break;
                case 7:
                    getworld.Add("Agrabah");
                    getworld.Add("agrabah");
                    break;
                case 8:
                    getworld.Add("Das Land der Drachen");
                    getworld.Add("landofdragons");
                    break;
                case 9:
                    getworld.Add("Hundertmorgenwald");
                    getworld.Add("100acre");
                    break;
                case 10:
                    getworld.Add("Das Geweihte Land");
                    getworld.Add("pridelands");
                    break;
                case 11:
                    getworld.Add("Atlantica");
                    getworld.Add("atlantica");
                    break;
                case 12:
                    getworld.Add("Schloss Disney");
                    getworld.Add("disney");
                    break;
                case 13:
                    getworld.Add("Fluss der Nostalgie");
                    getworld.Add("timelessriver");
                    break;
                case 14:
                    getworld.Add("Halloween Town");
                    getworld.Add("halloweentown");
                    break;
                case 15:
                    getworld.Add("Weltkarte");
                    getworld.Add("worldmap");
                    break;
                case 16:
                    getworld.Add("Port Royal");
                    getworld.Add("portroyal");
                    break;
                case 17:
                    getworld.Add("Space Paranoids");
                    getworld.Add("spaceparanoids");
                    break;
                case 18:
                    getworld.Add("The World That Never Was");
                    getworld.Add("twtnw");
                    break;
                case 255:
                    getworld.Add("Titelbild");
                    getworld.Add("logo");
                    break;
                default:
                    getworld.Add("Undefined location");
                    getworld.Add("logo");
                    break;
            }
            return getworld.ToArray();
        }
    }
}
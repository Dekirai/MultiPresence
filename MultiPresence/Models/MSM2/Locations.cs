namespace MultiPresence.Models.MSM2
{
    public class Locations
    {
        public static async Task<string> GetLocations(int location)
        {
            string getlocation;

            switch (location)
            {
                case 0:
                    getlocation = "Midtown";
                    break;
                case 1:
                    getlocation = "Hell's Kitchen";
                    break;
                case 2:
                    getlocation = "Greenwich";
                    break;
                case 3:
                    getlocation = "Chinatown";
                    break;
                case 4:
                    getlocation = "Financial District";
                    break;
                case 5:
                    getlocation = "Harlem";
                    break;
                case 6:
                    getlocation = "Upper East Side";
                    break;
                case 7:
                    getlocation = "Upper West Side";
                    break;
                case 8:
                    getlocation = "Central Park";
                    break;
                case 11:
                    getlocation = "Astoria";
                    break;
                case 12:
                    getlocation = "Downtown Queens";
                    break;
                case 13:
                    getlocation = "Little Odessa";
                    break;
                case 14:
                    getlocation = "Williamsburg";
                    break;
                case 15:
                    getlocation = "Downtown Brooklyn";
                    break;
                case 255:
                    getlocation = "Quest";
                    break;

                default:
                    getlocation = "Unknown location";
                    break;
            }
            return getlocation;
        }
    }
}
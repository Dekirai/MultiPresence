namespace MultiPresence.Models.TBOI
{
    public class FloorAssets
    {
        public static async Task<string> GetFloorAsset(int floor)
        {
            string getfloor;

            switch (floor)
            {
                case 0:
                    getfloor = "floor_basement";
                    break;
                case 1:
                    getfloor = "floor_cellar";
                    break;
                case 2:
                    getfloor = "floor_burningbasement";
                    break;
                case 3:
                    getfloor = "floor_caves";
                    break;
                case 4:
                    getfloor = "floor_downpour";
                    break;
                case 5:
                    getfloor = "floor_dross";
                    break;
                case 6:
                    getfloor = "floor_depths";
                    break;
                case 7:
                    getfloor = "floor_necropolis";
                    break;
                case 8:
                    getfloor = "floor_dankdepths";
                    break;
                case 9:
                    getfloor = "floor_womb";
                    break;
                case 10:
                    getfloor = "floor_womb";
                    break;
                case 11:
                    getfloor = "floor_scarredwomb";
                    break;
                case 12:
                    getfloor = "floor_hush";
                    break;
                case 13:
                    getfloor = "floor_sheol";
                    break;
                case 14:
                    getfloor = "floor_cathedral";
                    break;
                case 15:
                    getfloor = "floor_darkroom";
                    break;
                case 16:
                    getfloor = "floor_chest";
                    break;
                case 30:
                    getfloor = "floor_mausoleum";
                    break;
                case 31:
                    getfloor = "floor_gehenna";
                    break;
                case 32:
                    getfloor = "floor_mother";
                    break;
                case 34:
                    getfloor = "floor_home";
                    break;

                default:
                    getfloor = "logo";
                    break;
            }
            return getfloor;
        }
    }
}
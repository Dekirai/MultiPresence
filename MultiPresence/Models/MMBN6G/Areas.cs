namespace MultiPresence.Models.MMBN6G
{
    public class Areas
    {
        public static async Task<string[]> GetArea(int area)
        {
            List<string> getarea = new List<string>();
            switch (area)
            {
                case 0:
                    getarea.Add("ACDC Town");
                    getarea.Add("Class 6-A");
                    getarea.Add("");
                    break;
                case 1:
                    getarea.Add("Central Town");
                    getarea.Add("Lan's House");
                    getarea.Add("Lan's Room");
                    break;
                case 2:
                    getarea.Add("Class 6-1");
                    getarea.Add("Class 6-2");
                    getarea.Add("Class 1-1");
                    getarea.Add("Class 1-2");
                    getarea.Add("1F Hallway");
                    getarea.Add("2F Hallway");
                    getarea.Add("Foyer Hall");
                    getarea.Add("Teachers' Room");
                    break;
                case 128:
                    getarea.Add("RobCtrlComp1");
                    getarea.Add("RobCtrlComp2");
                    break;
                case 136:
                    getarea.Add("Lan's HP");
                    break;
                case 140:
                    getarea.Add("");
                    getarea.Add("");
                    getarea.Add("Class6-1Comp");
                    break;
                case 144:
                    getarea.Add("CentralArea1");
                    getarea.Add("CentralArea2");
                    break;


                default:
                    getarea.Add("Unknown Map");
                    break;
            }
            return getarea.ToArray();
        }
    }
}

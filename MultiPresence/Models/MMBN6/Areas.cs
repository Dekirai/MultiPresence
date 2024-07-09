namespace MultiPresence.Models.MMBN6
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
                    break;
                case 1:
                    getarea.Add("Central Town");
                    getarea.Add("Lan's House");
                    getarea.Add("Lan's Room");
                    getarea.Add("Bathroom");
                    getarea.Add("AsterLand  ");
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
                    getarea.Add("Principal's Office");
                    break;
                case 3:
                    getarea.Add("Seaside Town");
                    getarea.Add("Aquarium1");
                    getarea.Add("Aquarium2");
                    getarea.Add("Auditorium");
                    getarea.Add("Control Room");
                    break;
                case 4:
                    getarea.Add("Green Town");
                    getarea.Add("Court Foyer");
                    getarea.Add("Courtroom");
                    getarea.Add("PunishmentRm");
                    getarea.Add("UndrgrndRoom");
                    break;
                case 5:
                    getarea.Add("Admin");
                    getarea.Add("Sky Town");
                    getarea.Add("OpRoom");
                    getarea.Add("Force Room");
                    break;
                case 6:
                    getarea.Add("Expo Site");
                    getarea.Add("Central Hall");
                    getarea.Add("Seaside Hall");
                    getarea.Add("Green Hall");
                    getarea.Add("Sky Hall");
                    getarea.Add("CopyBot's Room");
                    break;
                case 128:
                    getarea.Add("RobCtrlComp1");
                    getarea.Add("RobCtrlComp2");
                    break;
                case 129:
                    getarea.Add("AquarumComp1");
                    getarea.Add("AquarumComp2");
                    getarea.Add("AquarumComp3");
                    break;
                case 130:
                    getarea.Add("JdgTreeComp1");
                    getarea.Add("JdgTreeComp2");
                    getarea.Add("JdgTreeComp3");
                    break;
                case 131:
                    getarea.Add("MrWearhrCmp1");
                    getarea.Add("MrWearhrCmp2");
                    getarea.Add("MrWearhrCmp3");
                    break;
                case 132:
                    getarea.Add("Stg6Dungeon1");
                    getarea.Add("Stg6Dungeon2");
                    getarea.Add("Stg6Dungeon3");
                    break;
                case 133:
                    getarea.Add("PavilonComp1");
                    getarea.Add("PavilonComp2");
                    getarea.Add("PavilonComp3");
                    getarea.Add("PavilonComp4");
                    getarea.Add("CopyBotComp");
                    break;
                case 136:
                    getarea.Add("Lan's HP");
                    getarea.Add("ACDC HP");
                    getarea.Add("Extra");
                    getarea.Add("Aquarium HP");
                    getarea.Add("Extra");
                    getarea.Add("Green HP");
                    getarea.Add("Sky HP");
                    break;
                case 140:
                    getarea.Add("RoboDogComp");
                    getarea.Add("Lab'sComp1");
                    getarea.Add("Class6-1Comp");
                    getarea.Add("Class6-2Comp");
                    getarea.Add("Class1-1Comp");
                    getarea.Add("Class1-2Comp");
                    getarea.Add("BathroomComp");
                    getarea.Add("ElevatorComp");
                    getarea.Add("FshStkSpComp");
                    getarea.Add("SecurCamComp");
                    getarea.Add("BookComp");
                    getarea.Add("Fan Comp");
                    getarea.Add("AirCndtrComp");
                    getarea.Add("Heater Comp");
                    getarea.Add("Shower Comp");
                    getarea.Add("HeliportComp");
                    getarea.Add("Lab'sComp2");
                    getarea.Add("VendngMcComp");
                    getarea.Add("PunshChrComp");
                    getarea.Add("WaterMchComp");
                    getarea.Add("Symbol Comp");
                    getarea.Add("Monitor Comp");
                    getarea.Add("PopcrnShpCmp");
                    getarea.Add("TeachrRmComp");
                    getarea.Add("Pipe Comp");
                    getarea.Add("ObservtnComp");
                    getarea.Add("OxygnTnkComp");
                    getarea.Add("PrcplOfcComp");
                    getarea.Add("Mascot Comp");
                    getarea.Add("StfToySpComp");
                    getarea.Add("DogHouseComp");
                    getarea.Add("GuidPanlComp");
                    break;
                case 144:
                    getarea.Add("CentralArea1");
                    getarea.Add("CentralArea2");
                    getarea.Add("CentralArea3");
                    break;
                case 145:
                    getarea.Add("SeasideArea1");
                    getarea.Add("SeasideArea2");
                    getarea.Add("SeasideArea3");
                    break;
                case 146:
                    getarea.Add("GreenArea1");
                    getarea.Add("GreenArea2");
                    break;
                case 147:
                    getarea.Add("Underground1");
                    getarea.Add("Underground2");
                    break;
                case 148:
                    getarea.Add("SkyArea1");
                    getarea.Add("SkyArea2");
                    getarea.Add("ACDCArea");
                    break;
                case 149:
                    getarea.Add("Undernet1");
                    getarea.Add("Undernet0");
                    getarea.Add("Undernet2");
                    getarea.Add("Undernet3");
                    break;
                case 150:
                    getarea.Add("Graveyard1");
                    getarea.Add("Graveyard2");
                    getarea.Add("ImmportalArea");
                    break;

                default:
                    getarea.Add("Unknown Map");
                    break;
            }
            return getarea.ToArray();
        }
    }
}

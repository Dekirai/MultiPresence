﻿namespace MultiPresence.Models.TY
{
    public class Levels
    {
        public static async Task<string[]> GetLevel(int level)
        {
            List<string> getlevel = new List<string>();
            switch (level)
            {
                case 0:
                    getlevel.Add("Rainbow Cliffs");
                    break;
                case 4:
                    getlevel.Add("Two Up");
                    break;
                case 5:
                    getlevel.Add("Walk in the Park");
                    break;
                case 6:
                    getlevel.Add("Ship Rex");
                    break;
                case 7:
                    getlevel.Add("Bull's Pen");
                    break;
                case 8:
                    getlevel.Add("Bridge on the River TY");
                    break;
                case 9:
                    getlevel.Add("Snow Worries");
                    break;
                case 10:
                    getlevel.Add("Outback Safari");
                    break;
                case 12:
                    getlevel.Add("Lyre, Lyre Pants on Fire");
                    break;
                case 13:
                    getlevel.Add("Beyond the black Stump");
                    break;
                case 14:
                    getlevel.Add("Rex marks the Spot");
                    break;
                case 15:
                    getlevel.Add("Fluffy's Fjord");
                    break;
                case 16:
                    getlevel.Add("Credits");
                    break;
                case 17:
                    getlevel.Add("Cass' Crest");
                    break;
                case 19:
                    getlevel.Add("Crikey's Cove");
                    break;
                case 20:
                    getlevel.Add("Cass' Pass");
                    break;
                case 21:
                    getlevel.Add("Bonus World");
                    break;
                case 22:
                    getlevel.Add("Bonus World");
                    break;
                case 23:
                    getlevel.Add("Final Battle");
                    break;
            }
            return getlevel.ToArray();
        }
    }
}

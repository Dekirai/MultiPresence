namespace MultiPresence.Models.TY
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

        public static async Task<string[]> GetLevelDE(int level)
        {
            List<string> getlevel = new List<string>();
            switch (level)
            {
                case 0:
                    getlevel.Add("Regenbogenklippen");
                    break;
                case 4:
                    getlevel.Add("Alles Roger");
                    break;
                case 5:
                    getlevel.Add("Parkspaziergang");
                    break;
                case 6:
                    getlevel.Add("Rex Ahoi");
                    break;
                case 7:
                    getlevel.Add("Bullenbau");
                    break;
                case 8:
                    getlevel.Add("Die Brücke am TY");
                    break;
                case 9:
                    getlevel.Add("Lawinensicher");
                    break;
                case 10:
                    getlevel.Add("Hinterland-Safari");
                    break;
                case 12:
                    getlevel.Add("Leier, Leier, Vogeleier");
                    break;
                case 13:
                    getlevel.Add("Jenseits von Nirgendwo");
                    break;
                case 14:
                    getlevel.Add("Rex und Billig");
                    break;
                case 15:
                    getlevel.Add("Fluffys Fjord");
                    break;
                case 16:
                    getlevel.Add("Credits");
                    break;
                case 17:
                    getlevel.Add("Cass Gipfel");
                    break;
                case 19:
                    getlevel.Add("Mannomanns Meer");
                    break;
                case 20:
                    getlevel.Add("Cass' Reise");
                    break;
                case 21:
                    getlevel.Add("Bonuswelt");
                    break;
                case 22:
                    getlevel.Add("Bonuswelt");
                    break;
                case 23:
                    getlevel.Add("Endschlacht");
                    break;
            }
            return getlevel.ToArray();
        }
    }
}

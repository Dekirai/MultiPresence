namespace MultiPresence.Models.MSM2
{
    public static class GameText
    {
        public static string GetLocation(int location)
        {
            switch (location)
            {
                case 0: return "Midtown";
                case 1: return "Hell's Kitchen";
                case 2: return "Greenwich";
                case 3: return "Chinatown";
                case 4: return "Financial District";
                case 5: return "Harlem";
                case 6: return "Upper East Side";
                case 7: return "Upper West Side";
                case 8: return "Central Park";
                case 9: return "Queens";
                case 10: return "Brooklyn";
                case 11: return "Astoria";
                case 12: return "Downtown Queens";
                case 13: return "Little Odessa";
                case 14: return "Williamsburg";
                case 15: return "Downtown Brooklyn";
                default: return "Unknown Location";
            }
        }

        public static string GetCharacter(int character)
        {
            switch (character)
            {
                case 0:
                case 2:
                    return "Peter Parker";
                case 1:
                case 3:
                    return "Miles Morales";
                case 4: return "Mary Jane Watson";
                case 5: return "Venom";
                case 6: return "Hailey Cooper";
                default: return "";
            }
        }

        public static string GetCharacterIcon(int character)
        {
            switch (character)
            {
                case 0:
                case 2:
                    return "peter";
                case 1:
                case 3:
                    return "miles";
                default: return "logo";
            }
        }

        public static string GetDifficulty(int difficulty)
        {
            switch (difficulty)
            {
                case 0: return "Friendly Neighborhood";
                case 1: return "Friendly";
                case 2: return "Amazing";
                case 3: return "Spectacular";
                case 4: return "Ultimate";
                default: return "";
            }
        }
    }
}

namespace MultiPresence.Models.RDL
{
    public class Weapons
    {
        public static async Task<string> GetWeapon(int weapon)
        {
            string weapons;

            switch (weapon)
            {
                case 2:
                    weapons = "Dual Vipers";
                    break;
                case 3:
                    weapons = "Vulcan Cannon";
                    break;
                case 4:
                    weapons = "The Arbiter";
                    break;
                case 5:
                    weapons = "Fusion Rifle";
                    break;
                case 6:
                    weapons = "Hunter Mine Launcher";
                    break;
                case 7:
                    weapons = "B6-Obliterator";
                    break;
                case 8:
                    weapons = "Holoshield Launcher";
                    break;
                case 9:
                    weapons = "Mini-Turret Launcher";
                    break;
                case 10:
                    weapons = "The Harbinger";
                    break;
                case 14:
                    weapons = "Swingshot";
                    break;
                case 15:
                    weapons = "Scorpion Flail";
                    break;
                default:
                    weapons = "Unknown Weapon";
                    break;
            }

            return weapons;
        }
    }
}
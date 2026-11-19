namespace MultiPresence.Models.RE
{
    public class Weapons
    {
        public static async Task<string> GetWeapon(int weaponid)
        {
            string weapon;

            switch (weaponid)
            {
                case 0:
                    weapon = "Nothing";
                    break;
                case 1:
                case 2:
                case 66:
                    weapon = "Survival Knife";
                    break;
                case 3:
                    weapon = "Handgun";
                    break;
                case 4:
                    weapon = "Self Defense Gun";
                    break;
                case 5:
                    weapon = "Handgun (Infinite)";
                    break;
                case 6:
                    weapon = "Shotgun";
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                    weapon = "Grenade Launcher";
                    break;
                case 11:
                    weapon = "Magnum Revolver";
                    break;
                case 12:
                    weapon = "Flamethrower";
                    break;
                case 13:
                    weapon = "Assault Shotgun";
                    break;
                case 14:
                    weapon = "Rocket Launcher (Infinite)";
                    break;
                case 15:
                    weapon = "Barry's 44 Magnum";
                    break;
                case 112:
                    weapon = "Dagger";
                    break;
                case 113:
                    weapon = "Flash Grenade";
                    break;
                case 114:
                    weapon = "Stun Gun";
                    break;

                default:
                    weapon = "Unknown Weapon";
                    break;
            }

            return weapon;
        }
    }
}
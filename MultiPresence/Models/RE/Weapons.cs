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

        public static async Task<string> GetWeaponDE(int weaponid)
        {
            string weapon;

            switch (weaponid)
            {
                case 1:
                    weapon = "Handgranate";
                    break;
                case 2:
                    weapon = "Brandgranate";
                    break;
                case 3:
                    weapon = "Matilda";
                    break;
                case 10:
                    weapon = "Goldenes Ei";
                    break;
                case 14:
                    weapon = "Blendgranate";
                    break;
                case 16:
                    weapon = "Armbrust";
                    break;
                case 33:
                case 34:
                case 64:
                case 256:
                    weapon = "Punisher";
                    break;
                case 35:
                case 36:
                case 257:
                    weapon = "Handgun";
                    break;
                case 37:
                case 38:
                    weapon = "Red9";
                    break;
                case 39:
                case 40:
                    weapon = "Blacktail";
                    break;
                case 41:
                    weapon = "Broken Butterfly";
                    break;
                case 42:
                case 43:
                    weapon = "Killer7";
                    break;
                case 44:
                case 258:
                    weapon = "Gewehr";
                    break;
                case 45:
                    weapon = "Striker";
                    break;
                case 46:
                case 107:
                    weapon = "Sturmgewehr";
                    break;
                case 47:
                case 108:
                    weapon = "Sturmgewehr (Halbauto.)";
                    break;
                case 48:
                case 50:
                    weapon = "TMP";
                    break;
                case 52:
                    weapon = "Chicago Typewriter";
                    break;
                case 53:
                    weapon = "Raketenwerfer";
                    break;
                case 54:
                case 259:
                case 261:
                    weapon = "Minenwerfer";
                    break;
                case 55:
                case 260:
                    weapon = "Handkanone";
                    break;
                case 65:
                    weapon = "P.R.L. 412";
                    break;
                case 71:
                    weapon = "Gewehr";
                    break;
                case 82:
                    weapon = "Krausers Bogen";
                    break;
                case 109:
                    weapon = "Endloswerfer";
                    break;
                case 148:
                    weapon = "Schrotgewehr";
                    break;
                case 0:
                case 2048:
                    weapon = "Nichts";
                    break;

                default:
                    weapon = "Unknown Weapon";
                    break;
            }

            return weapon;
        }
    }
}
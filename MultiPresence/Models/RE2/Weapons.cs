namespace MultiPresence.Models.RE2
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
                    weapon = "Matilda";
                    break;
                case 2:
                    weapon = "M19";
                    break;
                case 3:
                    weapon = "JMB HP3";
                    break;
                case 4:
                    weapon = "Quickdraw Army Revolver";
                    break;
                case 7:
                    weapon = "MUP";
                    break;
                case 8:
                    weapon = "Broom HC";
                    break;
                case 9:
                    weapon = "SLS 60";
                    break;
                case 11:
                    weapon = "W-870";
                    break;
                case 21:
                    weapon = "MQ 11";
                    break;
                case 23:
                    weapon = "LE 5 ∞";
                    break;
                case 31:
                    weapon = "Lightning Hawk";
                    break;
                case 41:
                    weapon = "EMF Visualizer";
                    break;
                case 42:
                    weapon = "GM 79";
                    break;
                case 43:
                    weapon = "Chemical Flamethrower";
                    break;
                case 44:
                    weapon = "Spark Shot";
                    break;
                case 45:
                    weapon = "ATM-4";
                    break;
                case 46:
                    weapon = "Combat Knife";
                    break;
                case 47:
                    weapon = "Combat Knife ∞";
                    break;
                case 49:
                    weapon = "Anti-Tank Rocket";
                    break;
                case 50:
                    weapon = "Minigun";
                    break;
                case 65:
                    weapon = "Hand Grenade";
                    break;
                case 66:
                    weapon = "Flash Grenade";
                    break;
                case 82:
                    weapon = "Samurai Edge ∞";
                    break;
                case 83:
                case 84:
                case 85:
                    weapon = "Samurai Edge";
                    break;
                case 222:
                    weapon = "ATM-4 ∞";
                    break;
                case 242:
                    weapon = "Anti-Tank Rocket ∞";
                    break;
                case 252:
                    weapon = "Minigun ∞";
                    break;

                default:
                    weapon = "Nothing";
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
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
    }
}
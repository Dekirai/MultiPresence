namespace MultiPresence.Models.RE3
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
                    weapon = "G19 Handgun";
                    break;
                case 2:
                    weapon = "G18 Burst Handgun";
                    break;
                case 3:
                    weapon = "G18 Handgun";
                    break;
                case 4:
                    weapon = "Samurai Edge";
                    break;
                case 7:
                    weapon = "MUP Handgun ∞";
                    break;
                case 11:
                    weapon = "Shotgun";
                    break;
                case 21:
                    weapon = "CQBR Assault Rifle";
                    break;
                case 22:
                    weapon = "CQBR Assault Rifle ∞";
                    break;
                case 31:
                    weapon = ".44 AE Lightning Hawk";
                    break;
                case 32:
                    weapon = "RAI-DEN";
                    break;
                case 42:
                    weapon = "Grenade Launcher";
                    break;
                case 46:
                    weapon = "Combat Knife";
                    break;
                case 47:
                    weapon = "Survival Knife";
                    break;
                case 48:
                    weapon = "HOT DOGGER";
                    break;
                case 49:
                    weapon = "Rocket Launcher ∞";
                    break;
                case 65:
                    weapon = "Hand Grenade";
                    break;
                case 66:
                    weapon = "Flash Grenade";
                    break;

                default:
                    weapon = "Nothing";
                    break;
            }

            return weapon;
        }
    }
}

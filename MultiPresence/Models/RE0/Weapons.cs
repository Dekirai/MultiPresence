using System.Collections.Generic;

namespace MultiPresence.Models.RE0
{
    public static class Weapons
    {
        private static readonly Dictionary<int, string> Names = new Dictionary<int, string>
        {
            { 0, "None" },
            { 1, "None" },
            { 2, "Knife" },
            { 3, "Handgun" },
            { 4, "Handgun" },
            { 5, "Hunting Gun" },
            { 6, "Shotgun" },
            { 7, "Grenade Launcher" },
            { 8, "Grenade Launcher" },
            { 9, "Grenade Launcher" },
            { 10, "Magnum" },
            { 11, "Sub-Machinegun" },
            { 14, "Molotov Cocktail" },
            { 17, "Custom Handgun" },
            { 19, "Custom Handgun" },
            { 22, "Magnum Revolver" },
            { 23, "Rocket Launcher" }
        };

        public static string GetWeapon(int itemid)
        {
            return Names.TryGetValue(itemid, out string? weapon) ? weapon : string.Empty;
        }
    }
}

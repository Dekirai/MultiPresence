using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace MultiPresence.Models.RE3
{
    public static class Weapons
    {
        private static readonly Dictionary<int, string> Names = new Dictionary<int, string>
        {
            { 0, "Nothing" },
            { 1, "G19 Handgun" },
            { 11, "M3 Shotgun" },
            { 2, "G18 Handgun (Burst Model)" },
            { 21, "CQBR Assault Rifle" },
            { 22, "Infinite CQBR Assault Rifle" },
            { 3, "G18 Handgun" },
            { 31, ".44 AE Lightning Hawk" },
            { 32, "RAI-DEN" },
            { 4, "Samurai Edge" },
            { 42, "MGL Grenade Launcher" },
            { 45, "Rocket Launcher" },
            { 46, "Combat Knife" },
            { 47, "Survival Knife" },
            { 48, "HOT DOGGER" },
            { 49, "Infinite Rocket Launcher" },
            { 50, "Railgun" },
            { 65, "Hand Grenade" },
            { 66, "Flash Grenade" },
            { 7, "Infinite MUP Handgun" }
        };
        public static string GetName(int id)
        {
            return id == 0 ? "Nothing" : Names.GetValueOrDefault(id) ?? "Weapon " + id.ToString(CultureInfo.InvariantCulture);
        }

        public static Task<string> GetWeapon(int weaponid)
        {
            return Task.FromResult(GetName(weaponid));
        }
    }
}

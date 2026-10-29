using System.Globalization;

namespace MultiPresence.Models.KH02
{
    public static class Difficulties
    {
        public static string GetName(int value)
        {
            return value switch
            {
                0 => "Beginner",
                1 => "Standard",
                2 => "Proud",
                3 => "Critical",
                _ => value.ToString(CultureInfo.InvariantCulture),
            };
        }
    }
}

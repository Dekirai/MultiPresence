namespace MultiPresence.Models.REV2
{
    internal static class CampaignLocations
    {
        public static string GetName(int stage, IReadOnlyDictionary<int, string> labels)
        {
            // The save screen maps the current stage to an entry in m25_sys_area.
            int area = stage switch
            {
                100 or 500 => 1,
                101 or 151 or 501 => 2,
                102 or 152 => 4,
                153 => 5,
                154 or 600 => 6,
                // The stage catalog also identifies The Struggle's 611 as the ruins used by 250 and 711.
                250 or 611 or 711 => 8,
                201 or 251 or 610 => 9,
                203 or 253 or 306 or 701 => 10,
                301 or 351 or 700 => 16,
                352 or 620 => 17,
                300 => 18,
                254 or 303 or 400 => 25,
                354 or 630 => 26,
                451 => 27,
                452 => 28,
                455 => 29,
                453 => 31,
                454 => 32,
                _ => 0
            };
            return area == 0 ? "" : labels.GetValueOrDefault(area, "");
        }
    }
}

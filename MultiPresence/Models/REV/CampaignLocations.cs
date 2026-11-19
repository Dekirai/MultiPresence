namespace MultiPresence.Models.REV
{
    internal static class CampaignLocations
    {
        public static string GetStage(int room)
        {
            // Rooms outside the ship map use the location captions of their stages (s1xx is the Queen Zenobia).
            return (room >> 8) switch
            {
                1 => "Queen Zenobia",
                2 => "Mediterranean Sea Coast",
                3 => "Valkoinen Mökki",
                4 => "Terragrigia",
                5 => "Queen Dido",
                _ => ""
            };
        }
    }
}

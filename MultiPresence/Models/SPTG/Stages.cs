namespace MultiPresence.Models.SPTG
{
    public static class Stages
    {
        public static string GetName(uint id)
        {
            return id switch
            {
                0x8A2A00A1 => "Frozen Suburbs",
                0x0C2BF7B2 => "Casa Loma's Film Set",
                0xDEF530CD => "Shopping District",
                0x50060FCB => "Casa Loma's Parking Lot",
                0x260CE4F0 => "Leo's Place",
                0x6910D204 => "The Main Stage",
                0x747EA326 => "Transit",
                0x22E2EC51 => "The Frying Tengu",
                0xB724FC23 => "Chengdu Gardens",
                0xA76FB1A6 => "Halloween Party",
                0x457996C9 => "Fire Escape",
                0x9138237F => "Dragons' Den",
                0x222FEF00 => "The Park",
                0x9F3F233B => "Eerie Forest",
                0x3CD17426 => "Chaos Theatre Elevator",
                0x46438BAD => "Subspace Highway?",
                0x468EF03F => "Final Stage: Techno Base",
                0x587A82EB => "Snowy Toronto",
                0x30263ED5 => "Ex Crossover",
                0xB52615CE or 0x1ED20022 or 0x3684272E or 0xF9471F1F => "Summertime Blues",
                0x2EEEE6D2 => "Battle at the Chaos Theatre",
                0x53CDA53D or 0x3F8C2624 => "Survival Horror",
                0x196D0354 or 0x90C1207A => "Boss Rush",
                0x06169FBC or 0xEBED4587 => "Battle Royal",
                0x406C0E99 or 0x46B6891D => "Dodge Ball",
                0xC7261FBB => "Joseph's Bedroom",
                0x029A9B8A => "Secret Level",
                0x783656F4 => "World Map",
                0x22373F6F => "Credits",
                0xD6BF3652 or 0x85A997B7 or 0x6F19B344 => "Main Menu",
                _ => "Unknown Stage"
            };
        }
    }
}

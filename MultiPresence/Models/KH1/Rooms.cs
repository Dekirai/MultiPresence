namespace MultiPresence.Models.KH1
{
    public class Rooms
    {
        public static async Task<string[]> GetRoom(string world)
        {
            List<string> getroom = new List<string>();

            switch (world)
            {
                case "Dive into the Heart":
                    getroom.Add("Disembark");
                    getroom.Add("Cinderalla Platform");
                    getroom.Add("Alice Platform");
                    getroom.Add("Awakening");
                    getroom.Add("Awakening");
                    getroom.Add("Destiny Islands");
                    getroom.Add("Destiny Islands");
                    break;

                case "Destiny Islands":
                    getroom.Add("Seashore");
                    getroom.Add("Seaside Shack");
                    getroom.Add("Cove");
                    getroom.Add("Seashore");
                    getroom.Add("Seaside Shack");
                    getroom.Add("Seashore");
                    getroom.Add("Seaside Shack");
                    getroom.Add("Secret Place");
                    getroom.Add("Island Remains");
                    getroom.Add("Sora's Room");
                    getroom.Add("Secret Place");
                    getroom.Add("Secret Place");
                    break;

                case "Disney Castle":
                    getroom.Add("Audience Chamber");
                    getroom.Add("Colonnade");
                    getroom.Add("Library");
                    getroom.Add("Courtyard");
                    getroom.Add("Spiral Stairs");
                    getroom.Add("Gummi Hangar");
                    getroom.Add("Outside Spiral Stairs Area");
                    getroom.Add("Path to the Crossroads");
                    getroom.Add("Disney Castle World");
                    break;

                case "Traverse Town":
                    getroom.Add("1st District");
                    getroom.Add("2nd District");
                    getroom.Add("3rd District");
                    getroom.Add("Vacant House");
                    getroom.Add("Alleyway");
                    getroom.Add("Green Room");
                    getroom.Add("Red Room");
                    getroom.Add("Hallway");
                    getroom.Add("Mystical House");
                    getroom.Add("Item Shop");
                    getroom.Add("Accessory Shop");
                    getroom.Add("Item Workshop");
                    getroom.Add("Geppetto's House");
                    getroom.Add("Dalmatians' Den");
                    getroom.Add("Dining Room");
                    getroom.Add("Living Room");
                    getroom.Add("Piano Room");
                    getroom.Add("Gizmo Shop");
                    getroom.Add("Merlin's House");
                    getroom.Add("Magician's Study");
                    getroom.Add("Magician's Lab");
                    getroom.Add("???");
                    getroom.Add("Secret Waterway");
                    getroom.Add("???");
                    getroom.Add("3rd District");
                    getroom.Add("Small House");
                    break;

                case "Wonderland":
                    getroom.Add("Rabitt Hole");
                    getroom.Add("Bizarre Room");
                    getroom.Add("Bizarre Room");
                    getroom.Add("Queen's Castle");
                    getroom.Add("Lotus Forest");
                    getroom.Add("Lotus Forest");
                    getroom.Add("Bizarre Room");
                    getroom.Add("Bizarre Room");
                    getroom.Add("Bizarre Room");
                    getroom.Add("Tea Party Garden");
                    break;

                case "Deep Jungle":
                    getroom.Add("Tree House");
                    getroom.Add("Camp");
                    getroom.Add("Bamboo Thicket");
                    getroom.Add("Jungle: Vines");
                    getroom.Add("Jungle: Vines 2");
                    getroom.Add("Hippos' Lagoon");
                    getroom.Add("Climbing Trees");
                    getroom.Add("Treetop");
                    getroom.Add("Jungle: Tunnel");
                    getroom.Add("Waterfall Cavern");
                    getroom.Add("Cavern of Hearts");
                    getroom.Add("Jungle: Cliff");
                    getroom.Add("Camp");
                    getroom.Add("Bamboo Thicket");
                    getroom.Add("Camp: Tent");
                    getroom.Add("Mini Game: Green Serpent");
                    getroom.Add("Mini Game: Splash Tunnel");
                    getroom.Add("Mini Game: Jade Spiral");
                    getroom.Add("Mini Game: Panic Fall");
                    getroom.Add("Mini Game: Shadow Cavern");
                    break;

                case "100 Acre Wood":
                    getroom.Add("Pooh's House");
                    getroom.Add("Pooh's Room");
                    getroom.Add("Rabbit's House");
                    getroom.Add("Rabbit's Room");
                    getroom.Add("Hunny Tree");
                    getroom.Add("Wood: Hill");
                    getroom.Add("Wood: Meadow");
                    getroom.Add("Bouncing Spot");
                    getroom.Add("Muddy Path");
                    getroom.Add("Wood: Hill");
                    getroom.Add("100 Acre Wood");
                    break;

                case "Agrabah":
                    getroom.Add("Desert");
                    getroom.Add("Desert: Cave");
                    getroom.Add("Agrabah: Plaza");
                    getroom.Add("Agrabah: Alley");
                    getroom.Add("Agrabah: Bazaar");
                    getroom.Add("Agrabah: Main Street");
                    getroom.Add("Palace Gates");
                    getroom.Add("Agrabah: Storage");
                    getroom.Add("Cave: Entrance");
                    getroom.Add("Cave: Hall");
                    getroom.Add("Bottomless Hall");
                    getroom.Add("Treasure Room");
                    getroom.Add("Relic Chamber");
                    getroom.Add("Dark Chamber");
                    getroom.Add("Silent Chamber");
                    getroom.Add("Hidden Room");
                    getroom.Add("Lamp Chamber");
                    getroom.Add("Cave: Core");
                    getroom.Add("Aladdin's House");
                    getroom.Add("Agrabah");
                    getroom.Add("Escaping from the Cave");
                    break;

                case "Atlantica":
                    getroom.Add("Atlantica");
                    getroom.Add("Tranquil Grotto");
                    getroom.Add("Calm Depths");
                    getroom.Add("Undersea Gorge");
                    getroom.Add("Undersea Cave");
                    getroom.Add("Undersea Garden");
                    getroom.Add("Sunken Ship");
                    getroom.Add("Below Deck");
                    getroom.Add("Sunken Ship");
                    getroom.Add("Den of Tides");
                    getroom.Add("Cavern Nook");
                    getroom.Add("Tidal Abyss");
                    getroom.Add("Ursula's Lair");
                    getroom.Add("Ariel's Grotto");
                    getroom.Add("Triton's Palace");
                    getroom.Add("Triton's Throne");
                    getroom.Add("Ursula Battle");
                    break;

                case "Halloween Town":
                    getroom.Add("Guillotine Square");
                    getroom.Add("Lab Entryway");
                    getroom.Add("Graveyard");
                    getroom.Add("Moonlight Hill");
                    getroom.Add("Bridge");
                    getroom.Add("Boneyard");
                    getroom.Add("Oogie's Manor");
                    getroom.Add("Torture Chamber");
                    getroom.Add("Manor Ruins");
                    getroom.Add("Evil Playroom");
                    getroom.Add("Research Lab");
                    getroom.Add("Guillotine Gate");
                    getroom.Add("Cemetary");
                    break;

                case "Olympus Coliseum":
                    getroom.Add("Coliseum Gates");
                    getroom.Add("Coliseum: Lobby");
                    getroom.Add("Coliseum: Arena");
                    getroom.Add("Main Gates");
                    getroom.Add("???");
                    getroom.Add("Coliseum: Arena");
                    getroom.Add("Coliseum: Arena");
                    break;

                case "Monstro":
                    getroom.Add("Monstro: Mouth");
                    getroom.Add("Monstro: Mouth");
                    getroom.Add("Monstro: Stomach");
                    getroom.Add("Monstro: Throat");
                    getroom.Add("Monstro: Bowels");
                    getroom.Add("Monstro: Chamber 1");
                    getroom.Add("Monstro: Chamber 2");
                    getroom.Add("Monstro: Chamber 3");
                    getroom.Add("Monstro: Chamber 4");
                    getroom.Add("Monstro: Chamber 5");
                    getroom.Add("Monstro: Chamber 6");
                    break;

                case "Neverland":
                    getroom.Add("Ship: Hold");
                    getroom.Add("Ship: Hold");
                    getroom.Add("Ship: Hold");
                    getroom.Add("Ship: Freezer");
                    getroom.Add("Ship: Galley");
                    getroom.Add("Ship: Cabin");
                    getroom.Add("Captain's Cabin");
                    getroom.Add("Ship: Hold");
                    getroom.Add("Pirate Ship");
                    getroom.Add("Clock Tower");
                    break;

                case "Hollow Bastion":
                    getroom.Add("Rising Falls");
                    getroom.Add("Castle Gates");
                    getroom.Add("Great Crest");
                    getroom.Add("High Tower");
                    getroom.Add("Entrance Hall");
                    getroom.Add("Library");
                    getroom.Add("Lift Stop");
                    getroom.Add("Base Level");
                    getroom.Add("Waterway");
                    getroom.Add("Waterway");
                    getroom.Add("Dungeon");
                    getroom.Add("Castle Chapel");
                    getroom.Add("Castle Chapel");
                    getroom.Add("Castle Chapel");
                    getroom.Add("Grand Hall");
                    getroom.Add("Dark Depths");
                    getroom.Add("Castle Chapel");
                    break;

                case "End of the World":
                    getroom.Add("Gate to the Dark");
                    getroom.Add("Final Dimension");
                    getroom.Add("Final Dimension");
                    getroom.Add("Final Dimension");
                    getroom.Add("Final Dimension");
                    getroom.Add("Final Dimension");
                    getroom.Add("Final Dimension");
                    getroom.Add("Final Dimension");
                    getroom.Add("Final Dimension");
                    getroom.Add("Final Dimension");
                    getroom.Add("Final Dimension");
                    getroom.Add("Final Dimension");
                    getroom.Add("Final Dimension");
                    getroom.Add("Dark Sphere");
                    getroom.Add("Giant Crevasse");
                    getroom.Add("World Terminus");
                    getroom.Add("World Terminus (Traverse Town)");
                    getroom.Add("World Terminus (Wonderland)");
                    getroom.Add("World Terminus (Olympus Coliseum)");
                    getroom.Add("World Terminus (Deep Jungle)");
                    getroom.Add("World Terminus (Agrabah)");
                    getroom.Add("World Terminus (Atlantica)");
                    getroom.Add("World Terminus (Halloween Town)");
                    getroom.Add("World Terminus (Neverland)");
                    getroom.Add("World Terminus (100 Acre Wood)");
                    getroom.Add("World Terminus (Hollow Bastion)");
                    getroom.Add("Evil Grounds");
                    getroom.Add("Volcanic Crater");
                    getroom.Add("Linked Worlds");
                    getroom.Add("Final Rest");
                    getroom.Add("Homecoming");
                    getroom.Add("Crumbling Island");
                    getroom.Add("Final Door");
                    getroom.Add("The Void");
                    getroom.Add("Crater");
                    getroom.Add("Homecoming");
                    getroom.Add("The Void");
                    getroom.Add("The Void");
                    getroom.Add("The Void");
                    break;

            }
            return getroom.ToArray();
        }
    }
}
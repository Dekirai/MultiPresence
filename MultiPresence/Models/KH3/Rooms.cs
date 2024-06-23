namespace MultiPresence.Models.KH3
{
    public class Rooms
    {
        public static async Task<string[]> GetRoom(string world)
        {
            List<string> getroom = new List<string>();

            switch (world)
            {
                case "World Map":
                    getroom.Add("World Map");
                    break;

                case "Kingdom of Corona":
                    getroom.Add("???");
                    getroom.Add("The Forest");
                    getroom.Add("The Kingdom");
                    break;

                case "Olympus":
                    getroom.Add("???");
                    getroom.Add("Mount Olympus");
                    getroom.Add("Thebes");
                    getroom.Add("Realm of the Gods");
                    break;

                case "Twilight Town":
                    getroom.Add("The Neighborhood");
                    break;

                case "Toy Box":
                    getroom.Add("Andy's House");
                    getroom.Add("Galaxy Toys");
                    getroom.Add("Verus Rex Minigame");
                    getroom.Add("UFO Battle Arena");
                    break;

                case "Dark World":
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    getroom.Add("");
                    break;

                case "Monstropolis":
                    getroom.Add("Monsters. Inc.");
                    getroom.Add("The Factory");
                    getroom.Add("The Powerplant");
                    getroom.Add("The Door Vault");
                    break;

                case "Main Menu":
                    getroom.Add("Main Menu");
                    break;
            }
            return getroom.ToArray();
        }
    }
}
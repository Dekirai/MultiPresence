namespace MultiPresence.Models.KHBBS
{
    public class Rooms
    {
        public static async Task<string[]> GetRoom(string world)
        {
            List<string> getroom = new List<string>();

            switch (world)
            {
                case "Main Menu":
                    getroom.Add("Main Menu");
                    break;

            }
            return getroom.ToArray();
        }
    }
}
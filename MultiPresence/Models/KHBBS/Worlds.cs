namespace MultiPresence.Models.KHBBS
{
    public class Worlds
    {
        public static async Task<string[]> GetWorld(int world)
        {
            List<string> getworld = new List<string>();

            //Order:
            //String to show on discord
            //String to use as imagekey

            switch (world)
            {
                default:
                    getworld.Add("Undefined location");
                    getworld.Add("logo");
                    break;
            }
            return getworld.ToArray();
        }
    }
}
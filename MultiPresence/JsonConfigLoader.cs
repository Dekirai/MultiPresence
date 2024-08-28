using Newtonsoft.Json.Linq;

namespace MultiPresence
{
    public class ConfigLoader
    {
        private JObject _config;

        public ConfigLoader(string configPath)
        {
            _config = LoadConfig(configPath);
        }

        private JObject LoadConfig(string configPath)
        {
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                return JObject.Parse(json);
            }
            else
            {
                Console.WriteLine($"Configuration file '{configPath}' not found.");
                return new JObject(); // Return an empty JObject or handle it as needed
            }
        }

        public JObject GetConfig()
        {
            return _config;
        }
    }
}
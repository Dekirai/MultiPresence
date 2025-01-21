using System.Text.Json;

namespace MultiPresence
{
    public class FileChecker
    {
        private static bool _isRunning = false;

        public static async Task EnsureFilesExistAsync()
        {
            if (_isRunning) return;
            _isRunning = true;
            try
            {
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                bool filesMissing = false;
                string[] filesToCheck = { "MultiPresenceGame.exe", "steam_api64.dll" };

                foreach (string file in filesToCheck)
                {
                    string filePath = Path.Combine(currentDirectory, file);
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"File not found: {file}");
                        filesMissing = true;
                    }
                }

                if (filesMissing)
                {
                    DialogResult result = MessageBox.Show(
                        "It seems you are running a game that requires Steamworks, do you want to download the missing files?\nThe file names are \"MultiPresenceGame.exe\" and \"steam_api64.dll\".",
                        "Warning",
                        MessageBoxButtons.YesNo
                    );

                    if (result == DialogResult.No)
                        return;
                }
                else
                {
                    return;
                }

                string apiUrl = $"https://api.github.com/repos/dekirai/MultiPresence/releases/latest";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "CSharp-Client");

                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(apiUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            string json = await response.Content.ReadAsStringAsync();
                            using (JsonDocument doc = JsonDocument.Parse(json))
                            {
                                JsonElement root = doc.RootElement;
                                JsonElement assets = root.GetProperty("assets");

                                foreach (JsonElement asset in assets.EnumerateArray())
                                {
                                    string name = asset.GetProperty("name").GetString();
                                    string downloadUrl = asset.GetProperty("browser_download_url").GetString();

                                    if (Array.Exists(filesToCheck, file => file == name))
                                    {
                                        HttpResponseMessage fileResponse = await client.GetAsync(downloadUrl);
                                        if (fileResponse.IsSuccessStatusCode)
                                        {
                                            byte[] fileBytes = await fileResponse.Content.ReadAsByteArrayAsync();
                                            string filePath = Path.Combine(currentDirectory, name);
                                            await File.WriteAllBytesAsync(filePath, fileBytes);
                                            Console.WriteLine($"Downloaded {name} successfully.");
                                        }
                                        else
                                        {
                                            MessageBox.Show($"Failed to download {name}. Status: {fileResponse.StatusCode}", "Error");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to fetch the latest release info. Status: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
            finally
            {
                _isRunning = false;
            }
        }
    }
}

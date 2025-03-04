using System.IO.Compression;
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
                string[] filesToCheck = { "MultiPresenceGame.exe", "steam_api64.dll" };

                bool filesMissing = false;
                foreach (string file in filesToCheck)
                {
                    if (!File.Exists(Path.Combine(currentDirectory, file)))
                        filesMissing = true;
                }

                if (!filesMissing) return;

                DialogResult result = MessageBox.Show(
                    "It seems you are running a game that requires Steamworks, do you want to download the missing files?\nThe file names are \"MultiPresenceGame.exe\" and \"steam_api64.dll\".",
                    "Warning",
                    MessageBoxButtons.YesNo
                );

                if (result == DialogResult.No)
                    return;

                string apiUrl = "https://api.github.com/repos/dekirai/MultiPresence/releases/latest";

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

                                    if (name == "Update.zip")
                                    {
                                        string zipPath = Path.Combine(currentDirectory, "Update.zip");

                                        HttpResponseMessage fileResponse = await client.GetAsync(downloadUrl);
                                        if (fileResponse.IsSuccessStatusCode)
                                        {
                                            byte[] fileBytes = await fileResponse.Content.ReadAsByteArrayAsync();
                                            await File.WriteAllBytesAsync(zipPath, fileBytes);

                                            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                                            {
                                                foreach (ZipArchiveEntry entry in archive.Entries)
                                                {
                                                    if (Array.Exists(filesToCheck, file => file == entry.Name))
                                                    {
                                                        string destinationPath = Path.Combine(currentDirectory, entry.Name);
                                                        entry.ExtractToFile(destinationPath, true);
                                                    }
                                                }
                                            }

                                            File.Delete(zipPath);
                                        }
                                        else
                                        {
                                            MessageBox.Show($"Failed to download Update.zip. Status: {fileResponse.StatusCode}", "Error");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Failed to fetch the latest release info. Status: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
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

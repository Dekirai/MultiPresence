using System.Diagnostics;
using System.Security.Policy;
using DiscordRPC;
using Memory;
using HtmlAgilityPack;

namespace MultiPresence.Presence
{
    public class OW
    {
        static Mem mem = new Mem();
        static string process = "Overwatch";
        private static DiscordRpcClient discord;
        private static string gameState = "";
        private static string gameName = "";
        private static string richPresence = "";
        private static DiscordStatusUpdater updater;

        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1270342180623487089");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            int pid = mem.GetProcIdFromName(process);
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                string steamId = "";
                var placeholders = new Dictionary<string, object>
                    {
                        { "steamId", steamId }
                    };
                steamId = updater.UpdateDetails("Overwatch", placeholders);
                string html = await FetchHtmlAsync($"https://steamcommunity.com/miniprofile/{steamId}?t={DateTime.Now}/");

                if (!string.IsNullOrEmpty(html))
                {
                    ExtractValuesFromHtml(html);
                }

                string state = updater.UpdateState("Overwatch", placeholders);
                discord.UpdateLargeAsset("logo", "Overwatch 2");
                discord.UpdateDetails(richPresence);
                discord.UpdateState(state);

                await Task.Delay(3000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                MainForm.gameUpdater.Start();
            }
        }

        static async Task<string> FetchHtmlAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string html = await response.Content.ReadAsStringAsync();
                    return html;
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        static void ExtractValuesFromHtml(string html)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);

            var gameStateNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='game_state']");
            var gameNameNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='miniprofile_game_name']");
            var richPresenceNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='rich_presence']");

            gameState = gameStateNode?.InnerText.Trim() ?? "N/A";
            gameName = gameNameNode?.InnerText.Trim() ?? "N/A";
            richPresence = richPresenceNode?.InnerText.Trim() ?? "In Menus";
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
            discord.SetPresence(new RichPresence()
            {
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}
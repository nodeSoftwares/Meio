using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordRPC;

namespace YouTubeMusicDiscordRPC
{
    class Program
    {
        private static DiscordRpcClient discordClient;
        private static string currentVideoId = "";
        private static MusicInfo currentMusicInfo;
        private const string CLIENT_ID = "1415069890766704700";

        static async Task Main(string[] args)
        {
            Console.Title = "YouTube Music Discord RPC";
            Console.WriteLine("🎵 Rich Presence YouTube Music pour Discord\n");

            InitializeDiscordRPC();

            Console.WriteLine("Instructions:");
            Console.WriteLine(" - tapez `download <url>` pour télécharger uniquement l'audio avec yt-dlp");
            Console.WriteLine(" - tapez `stop` pour arrêter le Rich Presence");
            Console.WriteLine(" - tapez `exit` pour quitter\n");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("🎵 Commande/URL: ");
                Console.ResetColor();

                string input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input)) continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                if (input.Equals("stop", StringComparison.OrdinalIgnoreCase))
                {
                    SetIdlePresence();
                    Console.WriteLine("❌ Rich Presence arrêté.");
                    continue;
                }

                if (input.StartsWith("download", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = input.Split(' ', 2);
                    if (parts.Length >= 2)
                    {
                        string url = parts[1];
                        await DownloadWithYtDlp(url);
                    }
                    continue;
                }

                Console.WriteLine("⚠️ Commande inconnue");
            }

            Cleanup();
        }

        // ================= Discord RPC =================
        static void InitializeDiscordRPC()
        {
            discordClient = new DiscordRpcClient(CLIENT_ID);

            discordClient.OnReady += (sender, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ Connecté à Discord en tant que {e.User.Username}");
                Console.ResetColor();
            };

            discordClient.OnError += (sender, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erreur Discord: {e.Message}");
                Console.ResetColor();
            };

            discordClient.OnConnectionFailed += (sender, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n❌ Connexion Discord échouée");
                Console.ResetColor();
            };

            discordClient.Initialize();
            SetIdlePresence();
        }

        static void SetIdlePresence()
        {
            discordClient.SetPresence(new RichPresence()
            {
                Details = "Aucune musique en cours",
                State = "En attente",
                Assets = new Assets()
                {
                    LargeImageKey = "youtube_music",
                    LargeImageText = "YouTube Music",
                    SmallImageKey = "idle",
                    SmallImageText = "En attente"
                }
            });

            currentVideoId = "";
            currentMusicInfo = null;
        }

        // ================= Téléchargement via yt-dlp =================
        static async Task DownloadWithYtDlp(string url)
        {
            try
            {
                string output = "%(title)s.%(ext)s";
                string args = $"-x --audio-format mp3 -o \"{output}\" {url}";

                var psi = new ProcessStartInfo
                {
                    FileName = "yt-dlp",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process == null)
                {
                    Console.WriteLine("❌ Impossible de démarrer yt-dlp");
                    return;
                }

                while (!process.StandardOutput.EndOfStream)
                {
                    string line = await process.StandardOutput.ReadLineAsync();
                    Console.WriteLine(line);
                }

                await process.WaitForExitAsync();
                Console.WriteLine(process.ExitCode == 0
                    ? "✅ Téléchargement terminé"
                    : "❌ Erreur lors du téléchargement avec yt-dlp");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception yt-dlp: {ex.Message}");
            }
        }

        // ================= Utilitaires =================
        static string ExtractVideoIdFromUrl(string url)
        {
            try
            {
                if (url.Contains("music.youtube.com"))
                {
                    var match = Regex.Match(url, @"music\.youtube\.com/watch\?v=([^&]+)");
                    if (match.Success) return match.Groups[1].Value;
                }

                if (url.Contains("youtu.be/"))
                    return url.Split("youtu.be/")[1].Split('?')[0];

                if (url.Contains("v="))
                {
                    var uri = new Uri(url);
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    return query["v"];
                }
            }
            catch { }
            return null;
        }

        static void Cleanup()
        {
            Console.WriteLine("\n🛑 Arrêt du Rich Presence...");
            discordClient?.ClearPresence();
            discordClient?.Dispose();
            Console.WriteLine("✅ Application arrêtée. Appuyez sur une touche pour fermer...");
            Console.ReadKey();
        }
    }

    public class MusicInfo
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public int Duration { get; set; }
    }
}

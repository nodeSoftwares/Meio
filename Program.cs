using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordRPC;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace YouTubeMusicDiscordRPC
{
    class Program
    {
        private static DiscordRpcClient discordClient;
        private static YoutubeClient youtubeClient;
        private static string currentVideoId = "";
        private static MusicInfo currentMusicInfo;
        private const string CLIENT_ID = "1415069890766704700";

        static async Task Main(string[] args)
        {
            Console.Title = "YouTube Music Discord RPC";
            Console.WriteLine("🎵 Rich Presence YouTube Music pour Discord");
            Console.WriteLine("=============================================\n");

            // Initialisation
            InitializeDiscordRPC();
            youtubeClient = new YoutubeClient();

            Console.WriteLine("Instructions:");
            Console.WriteLine("1. Ouvrez YouTube Music dans votre navigateur");
            Console.WriteLine("2. Copiez l'URL de la musique en cours");
            Console.WriteLine("3. Collez l'URL ici et appuyez sur Entrée");
            Console.WriteLine("4. Tapez 'stop' pour arrêter");
            Console.WriteLine("5. Tapez 'exit' pour quitter\n");

            // Boucle principale
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("🎵 Collez l'URL YouTube Music: ");
                Console.ResetColor();

                string input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                    continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (input.Equals("stop", StringComparison.OrdinalIgnoreCase))
                {
                    SetIdlePresence();
                    Console.WriteLine("❌ Rich Presence arrêté.");
                    continue;
                }

                if (input.Equals("clear", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Clear();
                    continue;
                }

                await ProcessYouTubeUrl(input);
            }

            Cleanup();
        }

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
                Console.WriteLine($"\n❌ Connexion Discord échouée");
                Console.ResetColor();
            };

            discordClient.Initialize();
            SetIdlePresence();
        }

        static async Task ProcessYouTubeUrl(string url)
        {
            try
            {
                if (!url.Contains("youtube.com") && !url.Contains("youtu.be"))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("⚠️  URL YouTube non valide");
                    Console.ResetColor();
                    return;
                }

                string videoId = ExtractVideoIdFromUrl(url);
                if (string.IsNullOrEmpty(videoId))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("⚠️  Impossible d'extraire l'ID de la vidéo");
                    Console.ResetColor();
                    return;
                }

                // Éviter de refaire la requête si c'est la même vidéo
                if (videoId == currentVideoId && currentMusicInfo != null)
                {
                    SetDiscordPresence(currentMusicInfo);
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("🔄 Récupération des informations de la musique...");
                Console.ResetColor();

                var video = await youtubeClient.Videos.GetAsync(videoId);
                
                currentMusicInfo = new MusicInfo
                {
                    VideoId = videoId,
                    Title = CleanTitle(video.Title),
                    Artist = video.Author.ChannelTitle,
                    Duration = video.Duration.HasValue ? (int)video.Duration.Value.TotalSeconds : 0,
                    ThumbnailUrl = GetThumbnailUrl(videoId),
                    Url = url
                };

                currentVideoId = videoId;

                SetDiscordPresence(currentMusicInfo);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ Musique mise à jour:");
                Console.WriteLine($"   Titre: {currentMusicInfo.Title}");
                Console.WriteLine($"   Artiste: {currentMusicInfo.Artist}");
                Console.WriteLine($"   Durée: {FormatTime(currentMusicInfo.Duration)}");
                Console.ResetColor();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erreur: {ex.Message}");
                Console.ResetColor();
                SetErrorPresence();
            }
        }

        static void SetDiscordPresence(MusicInfo musicInfo)
        {
            var presence = new RichPresence()
            {
                Details = $"{Truncate(musicInfo.Title, 128)}",
                State = $"Par {Truncate(musicInfo.Artist, 128)}",
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow
                },
                Assets = new Assets()
                {
                    LargeImageKey = musicInfo.ThumbnailUrl,
                    LargeImageText = $"YouTube Music - {FormatTime(musicInfo.Duration)}",
                    SmallImageKey = "music_icon",
                    SmallImageText = "En lecture sur YouTube Music"
                },
                Buttons = new Button[]
                {
                    new Button()
                    {
                        Label = "Écouter sur YouTube Music",
                        Url = musicInfo.Url
                    }
                }
            };

            discordClient.SetPresence(presence);
        }

        static void SetIdlePresence()
        {
            var presence = new RichPresence()
            {
                Details = "Aucune musique en cours",
                State = "En attente sur YouTube Music",
                Assets = new Assets()
                {
                    LargeImageKey = "youtube_music",
                    LargeImageText = "YouTube Music",
                    SmallImageKey = "idle",
                    SmallImageText = "En attente"
                }
            };

            discordClient.SetPresence(presence);
            currentVideoId = "";
            currentMusicInfo = null;
        }

        static void SetErrorPresence()
        {
            var presence = new RichPresence()
            {
                Details = "Erreur de lecture",
                State = "Vérifiez l'URL YouTube Music",
                Assets = new Assets()
                {
                    LargeImageKey = "error",
                    LargeImageText = "Erreur",
                    SmallImageKey = "warning",
                    SmallImageText = "Problème détecté"
                }
            };

            discordClient.SetPresence(presence);
        }

        // Méthodes utilitaires
        static string ExtractVideoIdFromUrl(string url)
        {
            try
            {
                if (url.Contains("youtu.be/"))
                {
                    return url.Split(new[] { "youtu.be/" }, StringSplitOptions.None)[1].Split('?')[0];
                }
                else if (url.Contains("v="))
                {
                    var uri = new Uri(url);
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    return query["v"];
                }
                else if (url.Contains("youtube.com/watch/"))
                {
                    return url.Split(new[] { "watch/" }, StringSplitOptions.None)[1].Split('?')[0];
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        static string GetThumbnailUrl(string videoId)
        {
            // Format standard des thumbnails YouTube
            return $"https://img.youtube.com/vi/{videoId}/maxresdefault.jpg";
        }

        static string CleanTitle(string title)
        {
            // Nettoyer le titre des tags communs
            string[] toRemove = { 
                "(Official Video)", "(Official Music Video)", "[Official Video]", 
                "(Official Audio)", "[Official Audio]", "(Lyrics)", "[Lyrics]",
                "(Lyric Video)", "[Lyric Video]", "| Official Video", "| Official Audio",
                "(Official HD Video)", "[HD]", "(HD)", "| HD", "(4K)", "[4K]", "| 4K",
                "(Clean Version)", "[Clean]", "(Explicit)", "[Explicit]"
            };

            foreach (var tag in toRemove)
            {
                title = title.Replace(tag, "");
            }

            return title.Trim(' ', '-', '|', '"', '\'');
        }

        static string FormatTime(int seconds)
        {
            if (seconds <= 0) return "0:00";
            
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return $"{(int)time.TotalMinutes}:{time.Seconds:00}";
        }

        static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
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
        public string ThumbnailUrl { get; set; }
        public string Url { get; set; }
    }
}
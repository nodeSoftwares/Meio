using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.Threading.Tasks;
using System.IO;

class youtube_integration
{
    static async Task Main(string[] args)
    {
        var youtube = new YoutubeClient();

        // ID ou URL de la vidéo
        var videoUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

        // Récupère les infos de la vidéo
        var video = await youtube.Videos.GetAsync(videoUrl);
        Console.WriteLine($"Titre: {video.Title}");

        // Récupère les flux disponibles (audio/vidéo)
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);

        // Choisit un flux vidéo avec audio (MP4 par exemple)
        var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

        // Télécharge dans un fichier
        if (streamInfo != null)
        {
            var filePath = $"{video.Title}.mp4";
            await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);
            Console.WriteLine($"Vidéo téléchargée : {filePath}");
        }
    }
}
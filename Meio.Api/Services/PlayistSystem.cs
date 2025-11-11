/*
    _____________
   < fuck nvidia >
    -------------
           \   ^__^
            \  (oo)\_______
               (__)\       )\/\
                   ||----w |
                   ||     ||
*/
namespace Meio.Api.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

public class VideoInfoDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public TimeSpan Duration { get; set; }
}

public class PlaylistProcessor
{
    private readonly YoutubeClient _youtube = new();

    // Récupère la liste des vidéos d'une playlist et retourne une liste d'infos simplifiées
    public async Task<List<VideoInfoDto>> GetPlaylistVideosAsync(string playlistUrl)
    {
        var videos = new List<VideoInfoDto>();
        var playlist = await _youtube.Playlists.GetAsync(playlistUrl);
        var playlistVideos = _youtube.Playlists.GetVideosAsync(playlist.Id);

        await foreach (var video in playlistVideos)
        {
            videos.Add(new VideoInfoDto
            {
                Id = video.Id.Value,
                Title = video.Title,
                Author = video.Author.ChannelTitle,
                Duration = video.Duration ?? TimeSpan.Zero
            });
        }
        return videos;
    }

    // Sauvegarde la playlist dans un fichier JSON, dans un dossier "Playlists"
    public async Task SavePlaylistToJsonAsync(string playlistName, List<VideoInfoDto> videos)
    {
        var folder = Path.Combine(Directory.GetCurrentDirectory(), "Playlists");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        // Nettoyage simple du nom de fichier
        var cleanName = string.Concat(playlistName.Split(Path.GetInvalidFileNameChars()));
        var filePath = Path.Combine(folder, $"{cleanName}.json");

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(videos, jsonOptions));
    }

    // Traitement complet d'une playlist : récupérer + sauvegarder
    public async Task ProcessPlaylistAsync(string playlistUrl)
    {
        var playlist = await _youtube.Playlists.GetAsync(playlistUrl);
        var videos = await GetPlaylistVideosAsync(playlistUrl);
        await SavePlaylistToJsonAsync(playlist.Title, videos);
    }

    // Gestion de plusieurs playlists en parallèle
    public async Task ProcessMultiplePlaylistsAsync(IEnumerable<string> playlistUrls)
    {
        var tasks = new List<Task>();
        foreach (var url in playlistUrls)
        {
            tasks.Add(ProcessPlaylistAsync(url));
        }
        await Task.WhenAll(tasks);
    }
}
/*usage :
 var processor = new PlaylistProcessor();

var playlists = new List<string>
{
    "https://youtube.com/playlist?list=PL3A_1s_Z8MQbYIvki-pbcerX8zrF4U8zQ",
};
await processor.ProcessMultiplePlaylistsAsync(playlists);

enfin je crois aze je suis trop une merde en C# (ca marche pas)
*/ 

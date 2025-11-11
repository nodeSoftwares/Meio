/*
 *  _____________________
   < why are you here ? >
    ---------------------
           \   ^__^
            \  (oo)\_______
               (__)\       )\/\
                   ||----w |
                   ||     ||
 */

using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Meio.Api.Services
{
    public class VideoDownloadService
    {
        private readonly YoutubeClient _youtube;

        public VideoDownloadService()
        {
            _youtube = new YoutubeClient();
        }

        // Nettoie un nom pour un fichier ou dossier valide
        private string CleanFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        // Récupère les infos d'une vidéo
        public async Task<(string Title, string Author, IVideoStreamInfo StreamInfo)> GetVideoInfoAsync(string url)
        {
            var video = await _youtube.Videos.GetAsync(url);
            var streams = await _youtube.Videos.Streams.GetManifestAsync(video.Id);
            var streamInfo = streams.GetMuxedStreams().GetWithHighestVideoQuality();

            return (
                Title: CleanFileName(video.Title),
                Author: CleanFileName(video.Author.Title),
                StreamInfo: streamInfo
            );
        }

        // Télécharge la vidéo dans le dossier organisé et retourne le chemin complet
        public async Task<string> DownloadVideoAsync(string url, string baseFolder)
        {
            var video = await _youtube.Videos.GetAsync(url);
            var streams = await _youtube.Videos.Streams.GetManifestAsync(video.Id);
            var streamInfo = streams.GetMuxedStreams().GetWithHighestVideoQuality();

            // Crée le dossier auteur si nécessaire
            string authorFolder = Path.Combine(baseFolder, CleanFileName(video.Author.Title));
            if (!Directory.Exists(authorFolder))
                Directory.CreateDirectory(authorFolder);

            // Chemin complet du fichier
            string fileName = $"{CleanFileName(video.Title)}.mp4";
            string filePath = Path.Combine(authorFolder, fileName);

            // Téléchargement
            await _youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);

            return filePath;
        }
    }
}

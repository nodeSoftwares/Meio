/*
 * Ne marche pas

using Meio.Api.Services.Meio.Api.Services;

namespace Meio.Api.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Meio.Api.Services;
}

namespace Meio.Api.Controllers
{
    public class DownloadController : ControllerBase
    {
        private readonly VideoDownloadService _downloadService;

        // Injection du service dans le controller via constructeur
        public DownloadController(VideoDownloadService downloadService)
        {
            _downloadService = downloadService;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetInfo([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("L'URL est requise.");

            try
            {
                var (title, author, streamInfo) = await _downloadService.GetVideoInfoAsync(url);
                var result = new
                {
                    Title = title,
                    Author = author,
                    VideoQuality = streamInfo.VideoQuality.Label,
                    Container = streamInfo.Container.Name
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des informations : {ex.Message}");
            }
        }

        [HttpPost("download")]
        public async Task<IActionResult> Download([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("L'URL est requise.");

            try
            {
                // Définit le dossier de base où stocker les vidéos (à adapter selon ton environnement)
                string downloadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");

                // Lance le téléchargement via le service
                var filePath = await _downloadService.DownloadVideoAsync(url, downloadsRoot);

                string fileName = Path.GetFileName(filePath);
                return PhysicalFile(filePath, "video/mp4", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors du téléchargement : {ex.Message}");
            }
        }
    }
}
 */
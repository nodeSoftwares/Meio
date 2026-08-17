using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Meio.Api.Interfaces.Services;

namespace Meio.Api.Services;

internal sealed class AudioMetadataService(IMeioLogger logger, IHttpClientFactory httpClientFactory) : IAudioMetadataService
{
    // https://musicbrainz.org/ws/2/
    // https://coverartarchive.org/release/(id)

    private const string CoverBaseUrl = "https://coverartarchive.org/release/";


    public async Task<string?> GetReleasePictureAsync(string releaseId, CancellationToken cancellationToken = default)
    {
        var releaseCoverUrl = $"{CoverBaseUrl}{releaseId}";

        var client = httpClientFactory.CreateClient("Meio");

        var response = await client.GetAsync(releaseCoverUrl, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);
            var deserializedResult = JsonSerializer.Deserialize<CoverArtArchiveResponse>(jsonString);

            logger.LogInformation("Retrieved cover art picture for release {releaseId}.", releaseId);
            return deserializedResult!.Images.First().ImageUrl;
        }

        logger.LogWarning("Could not retrieve cover art picture for release {releaseId}: {error}", releaseId, response.StatusCode);
        return null;
    }
}
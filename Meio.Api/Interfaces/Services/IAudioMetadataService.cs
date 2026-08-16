using System.Threading;
using System.Threading.Tasks;

namespace Meio.Api.Interfaces.Services;

public interface IAudioMetadataService
{
    public Task<string?> GetReleasePictureAsync(string releaseId, CancellationToken cancellationToken = default);
}
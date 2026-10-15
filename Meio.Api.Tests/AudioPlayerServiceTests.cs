using Meio.Api.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Meio.Api.Tests;

[TestClass]
public class AudioPlayerServiceTests
{
    private Mock<IHostApplicationLifetime> _lifetimeMock = null!;
    private IServiceProvider _provider = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var services = new ServiceCollection();

        _lifetimeMock = new Mock<IHostApplicationLifetime>();
        services.AddSingleton(_lifetimeMock.Object);

        services.AddLogging();
        services.AddHttpClient();
        services.AddMeioApi();

        _provider = services.BuildServiceProvider();
    }

    [TestMethod]
    public void Play_WithValidTrack_Succeeds()
    {
        var audioPlayerService = _provider.GetRequiredService<IAudioPlayerService>();

        // Copy a dummy audio file to temp for testing
        var tempAudioPath = Path.Combine(Path.GetTempPath(), "silent.wav");
        using (var stream = typeof(AudioPlayerServiceTests).Assembly.GetManifestResourceStream("Meio.Api.Tests.silent.wav"))
        using (var fileStream = File.Create(tempAudioPath))
        {
            stream?.CopyTo(fileStream);
        }

        // Play the media
        var media = audioPlayerService.Play(tempAudioPath);
        Assert.IsNotNull(media);

        // Temp cleanup
        if (File.Exists(tempAudioPath)) File.Delete(tempAudioPath);
    }

    [TestMethod]
    public void Play_WithInvalidTrack_Fails()
    {
        var audioPlayerService = _provider.GetRequiredService<IAudioPlayerService>();

        // Play a non-existent media
        var media = audioPlayerService.Play(string.Empty);

        Assert.IsNull(media);
    }
}
using LibVLCSharp.Shared;
using Meio.Api.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Meio.Api.Tests;

[TestClass]
public class AudioPlayerServiceTests
{
    private Mock<IHostApplicationLifetime> _lifetimeMock = null!;
    private IServiceProvider _provider = null!;

    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void TestInitialize()
    {
        var services = new ServiceCollection();

        _lifetimeMock = new Mock<IHostApplicationLifetime>();
        services.AddSingleton(_lifetimeMock.Object);

        services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.AddDebug();
            configure.SetMinimumLevel(LogLevel.Debug);
            configure.Services.Configure<AudioPlayerOptions>(options =>
            {
                options.LibVlcArguments = ["--aout=dummy"]; // uses a dummy audio output for headless systems.
            });
        });
        services.AddHttpClient();
        services.AddMeioApi();

        _provider = services.BuildServiceProvider();
    }

    [TestMethod]
    public async Task Play_WithValidTrack_Succeeds()
    {
        var audioPlayerService = _provider.GetRequiredService<IAudioPlayerService>();

        var tempAudioPath = GetTempAudio().Result;
        var mediaUri = new Uri(tempAudioPath);
        var media = audioPlayerService.Play(mediaUri);
        Assert.IsNotNull(media);

        await WaitUntilPlaying(media);

        Assert.AreEqual(VLCState.Playing, media.State);

        // Temp cleanup
        if (File.Exists(tempAudioPath)) File.Delete(tempAudioPath);
    }

    [TestMethod]
    public async Task Play_WithInvalidTrack_Fails()
    {
        var audioPlayerService = _provider.GetRequiredService<IAudioPlayerService>();

        // Play a non-existent media
        var media = audioPlayerService.Play(string.Empty);

        await Task.Delay(500, TestContext.CancellationToken); // The media will never be playing, so we manually add a 0.5s delay.

        Assert.IsNull(media); // The returned media is null if the service couldn't find the file.
    }

    [TestMethod]
    public async Task Play_WithValidHTTPUri_Succeeds()
    {
        var audioPlayerService = _provider.GetRequiredService<IAudioPlayerService>();

        var mediaUri = new Uri("https://www.w3schools.com/html/horse.mp3");
        var media = audioPlayerService.Play(mediaUri);
        Assert.IsNotNull(media);

        await WaitUntilPlaying(media);

        Assert.AreEqual(VLCState.Playing, media.State);
    }

    [TestMethod]
    public async Task Play_WithInvalidUri_Fails()
    {
        var audioPlayerService = _provider.GetRequiredService<IAudioPlayerService>();

        var nonExistentPath = Path.Combine(AppContext.BaseDirectory, "definitely_not_existing.wav");
        var mediaUri = new Uri(nonExistentPath);

        var media = audioPlayerService.Play(mediaUri);
        await Task.Delay(500, TestContext.CancellationToken); // The media will never be playing, so we manually add a 0.5s delay.

        Assert.IsNull(media);
    }

    [TestMethod]
    public async Task Play_WithValidFileUri_Succeeds()
    {
        var audioPlayerService = _provider.GetRequiredService<IAudioPlayerService>();

        var tempAudioPath = GetTempAudio().Result;
        var mediaUri = new Uri(tempAudioPath);
        var media = audioPlayerService.Play(mediaUri);
        Assert.IsNotNull(media);

        await WaitUntilPlaying(media);

        Assert.AreEqual(VLCState.Playing, media.State);

        if (File.Exists(tempAudioPath)) File.Delete(tempAudioPath);
    }

    [TestMethod]
    public async Task Play_OnMediaEnd_Stops()
    {
        var audioPlayerService = _provider.GetRequiredService<IAudioPlayerService>();

        var tempAudioPath = GetTempAudio().Result;
        var mediaUri = new Uri(tempAudioPath);
        var media = audioPlayerService.Play(mediaUri);
        Assert.IsNotNull(media);

        await WaitUntilPlaying(media);
        await Task.Delay((int)media.Duration, TestContext.CancellationToken); // Wait for the media's playback to end.

        Assert.AreEqual(VLCState.Ended, media.State);

        if (File.Exists(tempAudioPath)) File.Delete(tempAudioPath);
    }

    [TestMethod]
    public async Task Stop_WithMediaPlaying_Succeeds()
    {
        var audioPlayerService = _provider.GetRequiredService<IAudioPlayerService>();

        var tempAudioPath = GetTempAudio().Result;
        var mediaUri = new Uri(tempAudioPath);
        var media = audioPlayerService.Play(mediaUri);
        Assert.IsNotNull(media);

        await WaitUntilPlaying(media);

        audioPlayerService.Stop(); // Disposes media
        Assert.AreEqual(VLCState.Stopped, media.State);

        if (File.Exists(tempAudioPath)) File.Delete(tempAudioPath);
    }

    [TestMethod]
    public async Task Pause_WithMediaPlaying_Succeeds()
    {
        var audioPlayerService = _provider.GetRequiredService<IAudioPlayerService>();

        var tempAudioPath = GetTempAudio().Result;
        var mediaUri = new Uri(tempAudioPath);
        var media = audioPlayerService.Play(mediaUri);
        Assert.IsNotNull(media);

        await WaitUntilPlaying(media);
        // Add more delay to ensure it isn't trying to pause when the media still isn't considered as playing.
        await Task.Delay(200, TestContext.CancellationToken); 
        
        audioPlayerService.Pause();
        Assert.AreEqual(VLCState.Paused, media.State);

        audioPlayerService.Stop(); // Stops and disposes the media
        if (File.Exists(tempAudioPath)) File.Delete(tempAudioPath);
    }

    /// <summary>
    ///     Copies an embedded testing audio file to the Temp folder for usage.
    /// </summary>
    /// <returns>A new temp audio file's path.</returns>
    private async Task<string> GetTempAudio()
    {
        var tempAudioPath = Path.Combine(Path.GetTempPath(), $"silent_{Guid.NewGuid()}.wav");

        await using var stream = typeof(AudioPlayerServiceTests).Assembly.GetManifestResourceStream("Meio.Api.Tests.silent.wav");
        await using var fileStream = File.Create(tempAudioPath);

        if (stream is null) Assert.Fail("Could not create audio stream, audio file is probably missing.");
        await stream.CopyToAsync(fileStream, TestContext.CancellationToken);

        return tempAudioPath;
    }

    /// <summary>
    ///     Checks every 250ms if the media is playing.
    /// </summary>
    /// <param name="media">The media to check.</param>
    /// <param name="timeout">Time before it stops waiting in milliseconds.</param>
    private async Task WaitUntilPlaying(Media media, double timeout = 2250.0)
    {
        var startingTime = DateTime.Now;
        while (media.State == VLCState.Opening)
        {
            await Task.Delay(250, TestContext.CancellationToken);
            if (DateTime.Now - startingTime > TimeSpan.FromMilliseconds(timeout)) break;
        }
    }
}
using Meio.Api.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Meio.Api.Tests;

[TestClass]
public class AudioMetadataService
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

        services.AddLogging();
        services.AddHttpClient();
        services.AddMeioApi();

        _provider = services.BuildServiceProvider();
    }

    [TestMethod]
    public void GetReleasePictureAsync_WithValidReleaseId_Succeeds()
    {
        var audioMetadataService = _provider.GetRequiredService<IAudioMetadataService>();

        const string testReleaseId = "1610973c-e6b3-4e0b-808f-586e7aaa0d83"; // Jane! | The Long Faces
        var coverResult = audioMetadataService.GetReleasePictureAsync(testReleaseId, TestContext.CancellationToken);

        Console.WriteLine(coverResult.Result);
        Assert.IsNotNull(coverResult.Result);
    }

    [TestMethod]
    public void GetReleasePictureAsync_WithInvalidReleaseId_Fails()
    {
        var audioMetadataService = _provider.GetRequiredService<IAudioMetadataService>();

        var coverResult = audioMetadataService.GetReleasePictureAsync(string.Empty, TestContext.CancellationToken);

        Console.WriteLine(coverResult.Result);
        Assert.IsNull(coverResult.Result);
    }
}
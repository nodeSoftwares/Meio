using Meio.Api.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Meio.Api.Tests;

[TestClass]
public sealed class DependencyInjectionTests
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
    public void AddMeioApi_RegistersExpectedServices()
    {
        // Get services
        var audioPlayerService = _provider.GetService<IAudioPlayerService>();
        var audioMetadataService = _provider.GetService<IAudioMetadataService>();

        // Assert services
        Assert.IsNotNull(audioPlayerService);
        Assert.IsNotNull(audioMetadataService);
    }
}
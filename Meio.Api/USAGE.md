# Usage

**TO-DO**

```csharp
/* This demonstrates how to set up and use the Meio.Api with
 * - Dependency Injection
 * - Logging with PrettyConsole
 * - HTTP client configuration
 * - AudioPlayerService example usage
 *
 * We build a generic host, that will handle the container for dependencies,
 * manage the application's lifetime and provide a basic logging infrastructure.
 */
var host = Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
{
    // Configure logging via PrettyConsole
    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(LogLevel.Debug);
        builder.AddPrettyConsole(opt =>
        {
            opt.ShowLogLevel = true;
            opt.SingleLine = true;
            opt.IncludeScopes = true;
            opt.LogLevelCase = LogLevelCase.Upper;
        });
        builder.Services.Configure<AudioPlayerOptions>(options =>
        {
            // Here you can pass arguments to the LibVLC instance.
            options.LibVlcArguments = ["-v", "--network-caching=1000"]; // These arguments are examples.
        });
    });

    // Configure HTTP client for external API calls (MusicBrainz, etc.).
    services.AddHttpClient("Meio",
        client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Meio/1.0");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

    services.AddMeioApi(); // Services are registered here.
}).UseConsoleLifetime().Build();

host.Start(); // Start the host to initialize registered services.

// Resolve the audio player service.
var audioPlayerService = host.Services.GetRequiredService<IAudioPlayerService>();
// var metadataService = host.Services.GetRequiredService<IAudioMetadataService>();

Console.WriteLine("Playing example audio from URL");
audioPlayerService.Play(new Uri("https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3"));

// Waiting until graceful shutdown (CTRL+C)
Console.WriteLine("Press CTRL+C to exit...");
host.WaitForShutdown();
```
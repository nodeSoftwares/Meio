using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DiscordRPC;
using DiscordRPC.Logging;
using Microsoft.Extensions.Logging;
using LogLevel = DiscordRPC.Logging.LogLevel;

namespace Meio.app.Services;

public class DiscordService : IDisposable
{
    private readonly DiscordRpcClient? _client;
    private readonly TimeSpan _cooldown = TimeSpan.FromSeconds(16); // Time between calls.
    private readonly SemaphoreSlim _semaphore = new(1, 1); // Prevents multiple calls.
    private DateTime _lastUpdate = DateTime.MinValue;

    public DiscordService()
    {
        try
        {
            var appId = Environment.GetEnvironmentVariable("DISCORD_APP_ID");
            Console.WriteLine($"AppId: {appId}");
            Console.WriteLine($"Current Directory {Directory.GetCurrentDirectory()}");

            // Create the client and setup events.
            _client = new DiscordRpcClient(appId)
            {
                Logger = new ConsoleLogger(LogLevel.None) // Sadly, this is mandatory.
            };

            _client.OnReady += (sender, _) => App.Logger!.LogDebug("{object} | Initialized DiscordService.", sender);
            _client.OnClose += (sender, args) =>
                App.Logger!.LogError("{object} | There was an error connecting to discord. {args}", sender, args);
            _client.OnError += (sender, args) => App.Logger!.LogError("{object} | {args}", sender, args);

            //Connect to the RPC.
            _client.Initialize();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static DiscordService Instance { get; } = new(); // Static reference

    /// <summary>
    ///     To be called when done using.
    /// </summary>
    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
        App.Logger!.LogDebug("Disposed DiscordService.");
    }

    /// <summary>
    ///     Changes the current Presence, suitable for when the user is listening to music.
    /// </summary>
    /// <param name="title">Title of the single.</param>
    /// <param name="artist">Artist of the single.</param>
    /// <param name="album">Album the single is from.</param>
    /// <param name="albumArt">Album splash art.</param>
    /// <param name="timestamp">Length of the media duration (in seconds).</param>
    public async Task SetPresence(string title, string artist, string album, string albumArt, TimeSpan timestamp)
    {
        await _semaphore.WaitAsync();

        try
        {
            var timeSinceLastUpdate = DateTime.UtcNow - _lastUpdate;

            if (timeSinceLastUpdate < _cooldown)
            {
                var delay = _cooldown - timeSinceLastUpdate;
                App.Logger!.LogDebug("Rate Limited have to wait for {delay}s", delay.Seconds);
                await Task.Delay(delay); // Wait for cooldown to pass
                // TODO: RPC Queue system, if there is two RPCs in wait, only show the last one.
            }

            _client?.SetPresence(new RichPresence
            {
                Type = ActivityType.Listening,
                StatusDisplay = StatusDisplayType.Details,
                Details = title,
                State = $"{artist} - {album}",
                Timestamps = Timestamps.FromTimeSpan(timestamp),
                Assets = new Assets { LargeImageKey = albumArt },
                Buttons = [new Button { Label = "Join us!", Url = "https://github.com/nodeSoftwares/Meio/" }]
            });

            _lastUpdate = DateTime.UtcNow;

            App.Logger!.LogDebug("Changed discord presence to {title} | {artist} - {album}", title, artist, album);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    ///     Changes the current Presence, for general purpose.
    /// </summary>
    /// <param name="text">Title of the Presence.</param>
    /// <param name="state">State of the Presence.</param>
    public async Task SetPresence(string text, string state)
    {
        await _semaphore.WaitAsync();

        try
        {
            var timeSinceLastUpdate = DateTime.UtcNow - _lastUpdate;

            if (timeSinceLastUpdate < _cooldown)
            {
                var delay = _cooldown - timeSinceLastUpdate;
                await Task.Delay(delay); // Wait for cooldown to pass
            }

            _client?.SetPresence(new RichPresence
            {
                Details = text,
                State = state,
                Buttons = [new Button { Label = "Join us!", Url = "https://github.com/nodeSoftwares/Meio/" }]
            });

            _lastUpdate = DateTime.UtcNow;

            App.Logger!.LogDebug("Changed discord presence to {text} | {state}", text, state);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    ///     Clears the current Presence.
    /// </summary>
    public void ClearPresence()
    {
        _client?.ClearPresence();
    }
}
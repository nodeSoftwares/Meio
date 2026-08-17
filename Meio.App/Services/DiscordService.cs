using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DiscordRPC;
using Meio.Api.Interfaces.Services;
using Meio.app.Interfaces.Services;
using Microsoft.Extensions.Logging;
using TagLib;
using static System.GC;

namespace Meio.app.Services;

public class DiscordPresence : IDiscordService, IDisposable
{
    private readonly IAudioMetadataService _audioMetadataService;

    private readonly DiscordRpcClient? _client;

    private readonly TimeSpan _cooldown = TimeSpan.FromSeconds(16); // Time between calls.
    private readonly ILogger<App> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1); // Prevents multiple calls.
    private DateTime _lastUpdate = DateTime.MinValue;

    public DiscordPresence(ILogger<App> logger, IAudioMetadataService audioMetadataService)
    {
        _logger = logger;
        _audioMetadataService = audioMetadataService;

        _client = new DiscordRpcClient("1415069890766704700"); // Meio's App ID
        _client.Initialize();

        _client.OnReady += (_, _) => _logger.LogDebug("Discord RPC client initialized.");
        _client.OnClose += (_, _) => _logger.LogDebug("Discord RPC client closed.");
    }

    public bool IsConnected => _client != null;


    public async Task SetPresence(string text = "", string state = "Idling...")
    {
        if (!IsConnected)
            _logger.LogError("Tried setting presence, but Discord is not connected.");

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
                State = state
            });

            _lastUpdate = DateTime.UtcNow;

            _logger.LogDebug("Presence updated: {text} - {State}", text, state);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SetPresencePlay(File audioFile)
    {
        await _semaphore.WaitAsync();

        var audioTag = audioFile.Tag;
        var title = audioFile.Tag.Title;
        var artist = audioTag.Performers.FirstOrDefault();
        var album = audioFile.Tag.Album;
        var duration = audioFile.Properties.Duration;
        var coverUrl = audioTag.MusicBrainzReleaseId is not null
            ? _audioMetadataService.GetReleasePictureAsync(audioTag.MusicBrainzReleaseId)
            : null;

        try
        {
            var timeSinceLastUpdate = DateTime.UtcNow - _lastUpdate;

            if (timeSinceLastUpdate < _cooldown)
            {
                var delay = _cooldown - timeSinceLastUpdate;
                _logger.LogDebug("Rate Limited. Wait for {delay}s.", delay.Seconds);
                await Task.Delay(delay); // Wait for cooldown to pass
                // TODO: RPC Queue system, if there is two RPCs in wait, only show the last one.
            }

            _client?.SetPresence(new RichPresence
            {
                Type = ActivityType.Listening,
                StatusDisplay = StatusDisplayType.Details,
                Details = title,
                State = $"{album} - {artist}",
                Timestamps = Timestamps.FromTimeSpan(duration),
                Assets = coverUrl is not null ? new Assets { LargeImageKey = coverUrl.Result } : null
            });

            _lastUpdate = DateTime.UtcNow;

            _logger.LogDebug("Changed discord presence to {title} | {artist} - {album}", title, artist, album);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void ClearPresence()
    {
        if (!IsConnected)
            _logger.LogError("Tried clearing presence, but Discord is not connected.");

        _client?.ClearPresence();

        _logger.LogDebug("Cleared presence.");
    }

    public void Dispose()
    {
        Dispose(true);
        SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // TODO: Dispose DiscordService
    }
}
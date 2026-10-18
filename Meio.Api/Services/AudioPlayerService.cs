using System;
using System.IO;
using LibVLCSharp.Shared;
using Meio.Api.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Meio.Api.Services;

internal sealed class AudioPlayerService : IAudioPlayerService, IDisposable
{
    private readonly LibVLC? _libVlc;
    private readonly IMeioLogger _logger;
    private readonly MediaPlayer? _mediaPlayer;

    private bool _disposed;

    public AudioPlayerService(IOptions<AudioPlayerOptions> options, IMeioLogger logger, IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;

        try
        {
            _libVlc = new LibVLC(options.Value.LibVlcArguments);
            _mediaPlayer = new MediaPlayer(_libVlc);

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                _logger.LogInformation("Application stopping, disposing AudioPlayerService...");
                Dispose();
            });

            _logger.LogDebug("Initialized AudioPlayerService.");
        }
        catch (VLCException ex)
        {
            _logger.LogError("An error occured trying to initialize AudioPlayerService. {Exception}", ex.Message);
            _logger.LogInformation(
                "!! READ THIS !!\n If you are on a linux host, please make sure you have installed the libvlc library, you will not be able to read any audio otherwise !!!");
            Dispose();
        }
    }

    /// <summary>
    ///     Plays the given audio file.
    /// </summary>
    /// <param name="audioFilePath">Audio file's path.</param>
    public Media? Play(string audioFilePath)
    {
        EnsureInitializedAndNotDisposed();

        try
        {
            if (_mediaPlayer!.IsPlaying)
                throw new InvalidOperationException("Cannot play media player. Already playing.");

            if (!File.Exists(audioFilePath))
                throw new FileNotFoundException("The given audio file doesn't exist.");

            var media = new Media(_libVlc!, audioFilePath);

            _mediaPlayer.Play(media);
            _logger.LogInformation("Playing media file {AudioFilePath}.", audioFilePath);

            _mediaPlayer.EndReached += (_, _) =>
            {
                _logger.LogDebug("Media playback ended.");
                _mediaPlayer.Stop();
            };

            _mediaPlayer.EncounteredError += OnVLCEncounteredError;

            return media;
        }
        catch (Exception e)
        {
            _logger.LogError("Cannot play audio file. {e}", e.Message);

            return null;
        }
    }

    /// <summary>
    ///     Starts playing the given audio file.
    /// </summary>
    /// <param name="audioUri">Audio file's Uri.</param>
    public Media? Play(Uri audioUri)
    {
        EnsureInitializedAndNotDisposed();

        try
        {
            if (_mediaPlayer!.IsPlaying)
                throw new InvalidOperationException("Cannot play media player. Already playing.");

            if (audioUri.Scheme == "file" && !File.Exists(audioUri.LocalPath))
                throw new FileNotFoundException("The given audio file doesn't exist.");

            var media = audioUri.IsFile
                ? new Media(_libVlc!, audioUri.LocalPath)
                : new Media(_libVlc!, audioUri.ToString(), FromType.FromLocation);
            
            _mediaPlayer.Play(media);
            _logger.LogInformation("Playing media file from {AudioUri}.", audioUri);

            _mediaPlayer.EndReached += (_, _) =>
            {
                _logger.LogDebug("Media playback ended.");
                _mediaPlayer.Stop();
            };

            _mediaPlayer.EncounteredError += OnVLCEncounteredError;

            return media;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot play audio Uri. {e}", e.Message);
            return null;
        }
    }

    /// <summary>
    ///     Stops the current reading audio file.
    /// </summary>
    public void Stop()
    {
        EnsureInitializedAndNotDisposed();

        if (!_mediaPlayer!.IsPlaying && _mediaPlayer.Media is null)
        {
            _logger.LogWarning("Cannot stop the media player. No media is playing.");
            return;
        }

        _mediaPlayer.Stop();
        _mediaPlayer.Media?.Dispose();
        _logger.LogDebug("Stopped media player.");
    }

    /// <summary>
    ///     Pauses the current reading audio file.
    /// </summary>
    public void Pause()
    {
        EnsureInitializedAndNotDisposed();

        _mediaPlayer!.Pause();
        _logger.LogDebug("Paused media player.");
    }

    /// <summary>
    ///     Changes the volume of the current reading audio file.
    /// </summary>
    /// <param name="newVolume">New volume of the media player.</param>
    public void ChangeVolume(int newVolume)
    {
        EnsureInitializedAndNotDisposed();

        try
        {
            _mediaPlayer!.Volume = newVolume;
            _logger.LogDebug("Changed audio volume to {NewAudioVolume}.", newVolume);
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured trying to change audio volume. {Exception}", e.Message);
        }
    }

    /// <summary>
    ///     Changes the playback speed of the current playing media.
    /// </summary>
    /// <param name="speed">New playback speed.</param>
    public void ChangePlaybackSpeed(float speed)
    {
        EnsureInitializedAndNotDisposed();

        if (!_mediaPlayer!.IsPlaying)
        {
            _logger.LogError("Cannot change audio rate, no media is playing.");
        }
        else
        {
            _mediaPlayer.SetRate(speed);
            _logger.LogDebug("Changed audio rate to {Speed}.", speed);
        }
    }

    /// <summary>
    ///     Mute the current reading audio file.
    /// </summary>
    public void Mute()
    {
        EnsureInitializedAndNotDisposed();

        _mediaPlayer!.Mute = true;
        _logger.LogDebug("Muted media.");
    }

    /// <summary>
    ///     Unmute the current reading audio file.
    /// </summary>
    public void Unmute()
    {
        EnsureInitializedAndNotDisposed();

        _mediaPlayer!.Mute = false;
        _logger.LogDebug("Unmuted media.");
    }

    /// <summary>
    ///     To be called when done using.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // Dispose AudioPlayerService.
        _logger.LogDebug("Disposed.");
    }

    ~AudioPlayerService()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _mediaPlayer?.Stop(); // Make sure it stops first to avoid potential unwanted behavior.
            _mediaPlayer?.Media?.Dispose(); // Dispose the loaded media.
        }

        _mediaPlayer?.Dispose();
        _libVlc?.Dispose();

        _disposed = true;
    }

    private void EnsureInitializedAndNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, "AudioPlayerService is disposed.");
        if (_libVlc is null || _mediaPlayer is null)
        {
            throw new NullReferenceException("AudioPlayerService is not initialized due to LibVLC failing to initialize.");
        }
    }

    private void OnVLCEncounteredError(object? obj, EventArgs args)
    {
        _logger.LogError("Media playback encountered an error.");
        if (_mediaPlayer is { IsPlaying: true }) _mediaPlayer.Stop();
        throw new Exception("Media playback encountered an error.");
    }
}
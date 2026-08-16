using System;
using LibVLCSharp.Shared;
using Meio.Api.Interfaces.Services;
using Microsoft.Extensions.Hosting;

// ReSharper disable UnusedMember.Global

namespace Meio.Api.Services;

internal class AudioPlayerService : IAudioPlayerService, IDisposable
{
    // ReSharper disable once InconsistentNaming
    private readonly LibVLC _libVLC;
    private readonly IMeioLogger _logger;
    private readonly MediaPlayer _mediaPlayer;

    private bool _disposed;

    public AudioPlayerService(IMeioLogger logger, IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;

        try
        {
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

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
    ///     Starts playing the given audio file.
    /// </summary>
    /// <param name="audioFilePath">Audio file path.</param>
    public Media? Play(string audioFilePath)
    {
        ObjectDisposedException.ThrowIf(_disposed, "AudioPlayerService is disposed.");

        try
        {
            if (_mediaPlayer.IsPlaying)
            {
                _logger.LogError("An audio file is already being played. Please stop it first.");
                return null;
            }

            var media = new Media(_libVLC, audioFilePath);

            _mediaPlayer.Play(media);
            _logger.LogInformation("Playing media file {AudioFilePath} .", audioFilePath);

            _mediaPlayer.EndReached += (_, _) =>
            {
                _logger.LogDebug("Media playback ended.");
                // _mediaPlayer.Stop()
            };

            return media;
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured trying to play the audio file. {e}", e.Message);

            return null;
        }

        throw new ObjectDisposedException(nameof(AudioPlayerService));
    }

    /// <summary>
    ///     Starts playing the given audio file.
    /// </summary>
    /// <param name="audioUri">Audio file Uri.</param>
    public Media? Play(Uri audioUri)
    {
        try
        {
            if (_mediaPlayer.IsPlaying)
            {
                _logger.LogError("An audio file is already being played. Please stop it first.");
                return null;
            }

            var media = new Media(_libVLC, audioUri.AbsolutePath, FromType.FromLocation);

            _mediaPlayer.Play(media);
            _logger.LogInformation("Playing media file from url {AudioFilePath}.", audioUri.AbsolutePath);

            return media;
        }
        catch (Exception e)
        {
            _logger.LogError("An error occured trying to play the audio file from url. {e}", e.Message);
            return null;
        }
    }

    /// <summary>
    ///     Stops the current reading audio file.
    /// </summary>
    public void Stop()
    {
        if (!_mediaPlayer.IsPlaying)
        {
            _logger.LogError("Cannot stop the media player. No media is playing.");
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
        _mediaPlayer.Pause();
        _logger.LogDebug("Paused media player.");
    }

    /// <summary>
    ///     Changes the volume of the current reading audio file.
    /// </summary>
    /// <param name="newVolume">New volume of the media player.</param>
    public void ChangeVolume(int newVolume)
    {
        try
        {
            _mediaPlayer.Volume = newVolume;
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
        if (!_mediaPlayer.IsPlaying)
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
        _mediaPlayer.Mute = true;
        _logger.LogDebug("Muted media.");
    }

    /// <summary>
    ///     Unmute the current reading audio file.
    /// </summary>
    public void Unmute()
    {
        _mediaPlayer.Mute = false;
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

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _mediaPlayer.Stop(); // Make sure it stops first to avoid potential unwanted behavior.
            _mediaPlayer.Media?.Dispose(); // Dispose the loaded media.
        }

        _mediaPlayer.Dispose();
        _libVLC.Dispose();

        _disposed = true;
    }
}
using System;
using LibVLCSharp.Shared;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global

namespace Meio.Api.Services;

public class AudioPlayerService : IDisposable
{
    // ReSharper disable once InconsistentNaming
    private readonly LibVLC _libVLC;
    private readonly MediaPlayer _mediaPlayer;

    public AudioPlayerService()
    {
        try
        {
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            Api.Logger!.LogDebug("Initialized AudioPlayerService.");
        }
        catch (VLCException ex)
        {
            Api.Logger!.LogCritical("An error occured trying to initialize AudioPlayerService. {Exception}", ex.Message);
            Api.Logger!.LogInformation(
                "!! READ THIS !!\n If you are on a linux host, please make sure you have installed the libvlc library, you will not be able to read any audio otherwise !!!");
            Environment.Exit(-1);
        }
    }

    /// <summary>
    ///     To be called when done using.
    /// </summary>
    public void Dispose()
    {
        _mediaPlayer.Stop(); // Make sure it stops first to avoid potential unwanted behaviour.
        _mediaPlayer.Dispose();
        Api.Logger!.LogTrace("Disposed media player.");

        _libVLC.Dispose();
        Api.Logger!.LogTrace("Disposed libVLC.");

        GC.SuppressFinalize(this); // Dispose AudioPlayerService.
        Api.Logger!.LogDebug("Disposed.");

        Environment.Exit(0); // Exit after this.
    }

    /// <summary>
    ///     Starts playing the given audio file.
    /// </summary>
    /// <param name="audioFilePath">Audio file path.</param>
    public void Play(string audioFilePath)
    {
        try
        {
            if (_mediaPlayer.IsPlaying)
            {
                Api.Logger!.LogError("An audio file is already being played. Please stop it first.");
                return;
            }

            var media = new Media(_libVLC, audioFilePath);

            _mediaPlayer.Play(media);
            Api.Logger!.LogInformation("Playing media file {AudioFilePath} .", audioFilePath);
        }
        catch (Exception e)
        {
            Api.Logger?.LogError("An error occured trying to play the audio file. {e}", e.Message);
        }
    }

    /// <summary>
    ///     Starts playing the given audio file.
    /// </summary>
    /// <param name="audioUri">Audio file Uri.</param>
    public void Play(Uri audioUri)
    {
        try
        {
            if (_mediaPlayer.IsPlaying)
            {
                Api.Logger!.LogError("An audio file is already being played. Please stop it first.");
                return;
            }

            var media = new Media(_libVLC, audioUri.AbsolutePath, FromType.FromLocation);

            _mediaPlayer.Play(media);
            Api.Logger!.LogInformation("Playing media file from url {AudioFilePath} .", audioUri.AbsolutePath);
        }
        catch (Exception e)
        {
            Api.Logger?.LogError("An error occured trying to play the audio file from url. {e}", e.Message);
        }
    }

    /// <summary>
    ///     Stops the current reading audio file.
    /// </summary>
    public void Stop()
    {
        if (!_mediaPlayer.IsPlaying)
        {
            Api.Logger!.LogError("Cannot stop the media player. No media is playing.");
        }

        _mediaPlayer.Stop();
        _mediaPlayer.Media?.Dispose();
        Api.Logger!.LogDebug("Stopped media player.");
    }

    /// <summary>
    ///     Pauses the current reading audio file.
    /// </summary>
    public void Pause()
    {
        _mediaPlayer.Pause();
        Api.Logger!.LogDebug("Paused media player.");
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
            Api.Logger!.LogTrace("Changed audio volume to {NewAudioVolume}.", newVolume);
        }
        catch (Exception e)
        {
            Api.Logger!.LogError("An error occured trying to change audio volume. {Exception}", e.Message);
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
            Api.Logger!.LogError("Cannot change audio rate, no media is playing.");
        }
        else
        {
            _mediaPlayer.SetRate(speed);
            Api.Logger!.LogTrace("Changed audio rate to {Speed}.", speed);
        }
    }

    /// <summary>
    ///     Mute the current reading audio file.
    /// </summary>
    public void Mute()
    {
        _mediaPlayer.Mute = true;
        Api.Logger!.LogDebug("Muted media.");
    }

    /// <summary>
    ///     Unmute the current reading audio file.
    /// </summary>
    public void Unmute()
    {
        _mediaPlayer.Mute = false;
        Api.Logger!.LogDebug("Unuted media.");
    }
}
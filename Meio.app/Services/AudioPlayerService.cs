using System;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using Microsoft.Extensions.Logging;

namespace Meio.app.Services;

public class AudioPlayerService
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
            App.Logger!.LogDebug("Initialized MediaPlayerService.");
        }
        catch (VLCException ex)
        {
            App.Logger!.LogCritical("{Exception}.", ex);
            App.Logger!.LogInformation(
                "!! READ THIS !!\n If you are on a linux host, please make sure you have installed the libvlc library, you will not be able to read any audio otherwise !!!");
            Environment.Exit(-1);
        }
    }

    /// <summary>
    ///     Starts playing the given audio file.
    /// </summary>
    /// <param name="audioFilePath">Audio file path.</param>
    public void Play(string audioFilePath)
    {
        try
        {
            var media = new Media(_libVLC, audioFilePath);

            _mediaPlayer.Play(media);
            App.Logger!.LogDebug("Playing media file {AudioFilePath} .", audioFilePath);
        }
        catch (Exception e)
        {
            App.Logger?.LogError("{exception}", e);
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
            var media = new Media(_libVLC, audioUri.AbsolutePath, FromType.FromLocation);

            _mediaPlayer.Play(media);
            App.Logger!.LogDebug("Playing media file from url {AudioFilePath} .", audioUri.AbsolutePath);
        }
        catch (Exception e)
        {
            App.Logger?.LogError("{exception}", e);
        }
    }

    /// <summary>
    ///     Stops the current reading audio file.
    /// </summary>
    public void Stop()
    {
        _mediaPlayer.Stop();
        App.Logger!.LogDebug("Stopped media player.");
    }

    /// <summary>
    ///     Pauses the current reading audio file.
    /// </summary>
    public void Pause()
    {
        _mediaPlayer.Pause();
        App.Logger!.LogDebug("Paused media player.");
    }

    /// <summary>
    ///     Changes the volume of the current reading audio file.
    /// </summary>
    /// <param name="newVolume">New volume of the media player.</param>
    public async void ChangeVolume(int newVolume)
    {
        await Dispatcher.UIThread.InvokeAsync(() => { _mediaPlayer.Volume = newVolume; });
        App.Logger!.LogTrace("Changed audio volume to {NewAudioVolume}.", newVolume);
    }

    /// <summary>
    ///     Mute the current reading audio file.
    /// </summary>
    public void Mute()
    {
        _mediaPlayer.Mute = true;
        App.Logger!.LogDebug("Muted media.");
    }

    /// <summary>
    ///     Unmute the current reading audio file.
    /// </summary>
    public void Unmute()
    {
        _mediaPlayer.Mute = false;
        App.Logger!.LogDebug("Unmuted media.");
    }
}
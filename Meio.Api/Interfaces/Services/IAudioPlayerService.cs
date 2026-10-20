using System;
using LibVLCSharp.Shared;

namespace Meio.Api.Interfaces.Services;

public interface IAudioPlayerService
{
    public Media? Play(string audioFilePath);

    public Media? Play(Uri audioUri);

    public void Stop();

    public void Pause();

    public void ChangeVolume(int newVolume);

    public void ChangePlaybackSpeed(float speed);

    public void Mute();

    public void Unmute();
}
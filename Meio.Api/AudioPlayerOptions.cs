using System;

namespace Meio.Api;

public class AudioPlayerOptions
{
    public string[] LibVlcArguments { get; set; } = Array.Empty<string>();
}
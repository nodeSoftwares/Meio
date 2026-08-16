using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Meio.Api.Interfaces.Services;
using Meio.app.Interfaces.Services;
using Meio.app.Services;
using Microsoft.Extensions.Logging;
using TagLib;

namespace Meio.app;

public partial class MainWindow : Window
{
    private readonly IAudioPlayerService _audioPlayerService;
    private readonly IDiscordService _discordService;
    private readonly ILogger _logger;
    private bool _debounce;
    private string? _filePath;
    private CancellationTokenSource? _volumeDebounceToken;

    // DIS WHOLE CODE IS HORIRBLE AAAAAAAAAA

    public MainWindow(ILogger<App> logger, IAudioPlayerService audioPlayerService, IDiscordService discordService)
    {
        _logger = logger;
        _audioPlayerService = audioPlayerService;
        _discordService = discordService;

        InitializeComponent();
        Task.Run(() => _discordService.SetPresence());
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!_debounce)
        {
            _debounce = true;

            if (_filePath == null) return;

            var metadata = File.Create(_filePath);
            if (metadata == null) return;

            _audioPlayerService.Play(_filePath);

            PlayButton.Content = "Stop";

            CurrentMusicText.Text = $"{metadata.Tag.Title} - {metadata.Tag.Performers.FirstOrDefault("unknown")}";
            AlbumArtImage.Source =
                metadata.Tag.Pictures != null ? ImageHelper.LoadBitmapFromBytes(metadata.Tag.Pictures.First().Data.Data) : null;

            Task.Run(() => _discordService.SetPresencePlay(metadata));
        }
        else
        {
            PlayButton.Content = "Play";
            CurrentMusicText.Text = "Nothing";
            AlbumArtImage.Source = null;

            _audioPlayerService.Stop();
            _debounce = false;
            Task.Run(() => _discordService.SetPresence());
        }
    }

    private void VolumeSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        _volumeDebounceToken?.Cancel();
        _volumeDebounceToken = new CancellationTokenSource();

        var token = _volumeDebounceToken.Token;
        var newVolume = (int)e.NewValue;
        VolumeText.Text = $"{newVolume * 100 / 30}%";

        Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100, token); // 100ms debounce
                    if (!token.IsCancellationRequested) _audioPlayerService.ChangeVolume(newVolume);
                }
                catch (TaskCanceledException)
                {
                    // Ignore
                }
            },
            token);
    }

    private async void UploadButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var url = await FileHelper.GetFilePathDialog(GetTopLevel(this));
            if (url != null) _filePath = Uri.UnescapeDataString(url.AbsolutePath);
        }
        catch (Exception exception)
        {
            _logger.LogError("There was an error trying to parse the URI unescape data. {exception}", exception.Message);
        }
    }
}
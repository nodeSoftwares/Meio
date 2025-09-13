using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Meio.app.Services;

namespace Meio.app;

public partial class MainWindow : Window
{
    private readonly AudioPlayerService _audioPlayerService;
    private bool _debounce;
    private string? _filePath;
    private CancellationTokenSource? _volumeDebounceToken;

    public MainWindow()
    {
        InitializeComponent();
        _audioPlayerService = new AudioPlayerService();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!_debounce)
        {
            _debounce = true;

            if (_filePath == null) return;

            var metadata = AudioMetadataService.LoadMetadata(_filePath);
            _audioPlayerService.Play(_filePath);

            PlayButton.Content = "Stop";
            CurrentMusicText.Text = $"{metadata.Title} - {metadata.Artists?[0]}";
            AlbumArtImage.Source = ImageHelper.LoadBitmapFromBytes(metadata.AlbumArt);
        }
        else
        {
            PlayButton.Content = "Play";
            CurrentMusicText.Text = "Nothing";
            AlbumArtImage.Source = null;

            _audioPlayerService.Stop();
            _debounce = false;
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
            // if (url != null) _filePath = Uri.UnescapeDataString(url.AbsolutePath);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Meio.app.Services;

namespace Meio.app;

public partial class MainWindow : Window
{
    private readonly MediaPlayerService _mediaPlayerService;
    private bool _debounce;
    private string? _filePath;
    private CancellationTokenSource? _volumeDebounceToken;

    public MainWindow()
    {
        InitializeComponent();
        _mediaPlayerService = new MediaPlayerService();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!_debounce)
        {
            _debounce = true;

            PlayButton.Content = "Stop";
            CurrentMusicText.Text = Path.GetFileName(_filePath)?.Split('.')[0]; // Set the audio file's name without the extension

            if (_filePath != null) _mediaPlayerService.Play(_filePath);
        }
        else
        {
            PlayButton.Content = "Play";
            CurrentMusicText.Text = "Nothing";

            _mediaPlayerService.Stop();
            _debounce = false;
        }
    }

    private void VolumeSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        _volumeDebounceToken?.Cancel();
        _volumeDebounceToken = new CancellationTokenSource();

        var token = _volumeDebounceToken.Token;
        var newVolume = (int)e.NewValue;

        Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100, token); // 100ms debounce
                    if (!token.IsCancellationRequested) _mediaPlayerService.ChangeVolume(newVolume);
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
            Console.WriteLine(exception);
        }
    }
}
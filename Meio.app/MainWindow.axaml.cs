using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using LibVLCSharp.Shared;

namespace Meio.app;

public partial class MainWindow : Window
{
    private readonly LibVLC _libVlc;
    private readonly MediaPlayer _mediaPlayer;
    private bool _debounce;
    private string _filePath;
    private CancellationTokenSource? _volumeDebounceToken;


    public MainWindow()
    {
        InitializeComponent();

        _libVlc = new LibVLC(true);
        _mediaPlayer = new MediaPlayer(_libVlc);
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!_debounce)
        {
            _debounce = true;
            Debug.WriteLine("Playing sound!");

            PlayButton.Content = "Stop";
            CurrentMusicText.Text = Path.GetFileName(_filePath).Split('.')[0];

            var media = new Media(_libVlc, _filePath);
            _mediaPlayer.Play(media);
        }
        else
        {
            PlayButton.Content = "Play";
            CurrentMusicText.Text = "?? - ??";

            _mediaPlayer.Stop();
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
                    if (!token.IsCancellationRequested)
                    {
                        // UI thread-safe call
                        await Dispatcher.UIThread.InvokeAsync(() => { _mediaPlayer.Volume = newVolume; });
                    }
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
            var url = await GetFileLinkDialog();
            if (url != null) _filePath = Uri.UnescapeDataString(url.AbsolutePath);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private async Task<Uri?> GetFileLinkDialog()
    {
        // Get a reference to the TopLevel/window where this control is hosted
        var topLevel = GetTopLevel(this);
        if (topLevel == null)
            return null;

        var provider = topLevel.StorageProvider;

        var options = new FilePickerOpenOptions
        {
            Title = "Select a File",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("All")
                {
                    Patterns = ["*.mp3", "*.wav"]
                }
            ]
        };

        var result = await provider.OpenFilePickerAsync(options);

        if (result.Count <= 0) return null;
        var file = result[0];
        var filePath = file.Path;

        return filePath;
    }
}
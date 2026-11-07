using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Meio.Api.Interfaces.Services;
using Meio.desktop.Interfaces.Services;
using Meio.desktop.Models;
using Microsoft.Extensions.Logging;

namespace Meio.desktop.Views;

public partial class MainWindow : Window
{
    private readonly IAudioPlayerService _audioPlayerService;
    private readonly IDiscordService _discordService;
    private readonly ILogger _logger;
    private readonly INavigationService _navigationService;

    // For the Avalonia Previewer
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public MainWindow(ILogger<App> logger, INavigationService navigationService, IAudioPlayerService audioPlayerService,
        IDiscordService discordService)
    {
        _logger = logger;
        _audioPlayerService = audioPlayerService;
        _discordService = discordService;
        _navigationService = navigationService;

        _navigationService.SetNavigator(page => MainContent.Content = page);
        Task.Run(() => _discordService.SetPresence());

        InitializeComponent();
        DataContext = this;

        _navigationService.Navigate<HomeView>();
        _logger.LogInformation("Meio is ready!!");
    }

    public ObservableCollection<PlaylistCardModel> DummyPlaylistCards { get; set; } =
    [
        new(new SolidColorBrush(Colors.Red)),
        new(new SolidColorBrush(Colors.DarkGoldenrod)),
        new(new SolidColorBrush(Colors.DodgerBlue)),
        new(new SolidColorBrush(Colors.LawnGreen)),
        new(new SolidColorBrush(Colors.DarkGoldenrod)),
        new(new SolidColorBrush(Colors.LightPink)),
        new(new SolidColorBrush(Colors.DarkGoldenrod)),
        new(new SolidColorBrush(Colors.AntiqueWhite))
    ];
}
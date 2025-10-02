using Avalonia.Controls;
using Meio.app.Services;
using Microsoft.Extensions.Logging;

namespace Meio.app.Views;

public partial class PlaylistView : UserControl, INavigable
{
    public PlaylistView()
    {
        InitializeComponent();
    }

    public void OnNavigateTo(object? parameter)
    {
        App.Logger!.LogTrace("Navigated to the playlist page.");
        // TODO
    }
}
using Avalonia.Controls;
using Meio.app.Services;
using Microsoft.Extensions.Logging;

namespace Meio.app.Views;

public partial class SettingsView : UserControl, INavigable
{
    public SettingsView()
    {
        InitializeComponent();
    }


    public void OnNavigateTo(object? parameter)
    {
        App.Logger!.LogTrace("Navigated to the settings page.");
    }
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.Extensions.Logging;

namespace Meio.app.Views;

public partial class MainWindow : Window
{
    private readonly bool _debounce;

    public MainWindow()
    {
        _debounce = false;
        InitializeComponent();
        // TODO: page NavigationService
    }

    private void Profile_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        App.Logger!.LogInformation("Profile_OnPointerPressed");
    }

    private void SettingsItem_OnClick(object? sender, RoutedEventArgs e)
    {
        App.Logger!.LogInformation("SettingsItem_OnClick");
    }

    private void QuitItem_OnClick(object? sender, RoutedEventArgs e)
    {
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow
            ?.Close(); // Exit the application.
    }

    private void Menu_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        App.Logger!.LogInformation("Menu_OnPointerPressed");

        // Only trigger on left-click
        if (!e.GetCurrentPoint((Visual)sender!).Properties.IsLeftButtonPressed) return;
        if (sender is not Avalonia.Svg.Skia.Svg { ContextMenu: { } menu } svg) return;
        menu.PlacementTarget = svg;
        menu.Placement = PlacementMode.RightEdgeAlignedTop; // Menu-style position
        menu.Open();
    }

    private void ToggleThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_debounce) (Application.Current as App)?.SwitchToLight();
        else (Application.Current as App)?.SwitchToDark();
    }
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Meio.app.Services;
using Microsoft.Extensions.Logging;

namespace Meio.app.Views;

public partial class MainWindow : Window
{
    private readonly NavigationService _navigationService;
    private bool _debounce;

    public MainWindow()
    {
        _debounce = false;
        InitializeComponent();

        _navigationService = new NavigationService(SetPage);
        _navigationService.Navigate<HomeView>(); // Set the homepage by default.
    }

    public void SetPage(UserControl page)
    {
        MainContent.Content = page;
    }

    private void Profile_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        App.Logger!.LogTrace("Profile_OnPointerPressed");
    }

    private void SettingsItem_OnClick(object? sender, RoutedEventArgs e)
    {
        App.Logger!.LogTrace("SettingsItem_OnClick");
        _navigationService.Navigate<SettingsView>();
    }

    private void QuitItem_OnClick(object? sender, RoutedEventArgs e)
    {
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow
            ?.Close(); // Exit the application.
    }

    private void Menu_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        App.Logger!.LogTrace("Menu_OnPointerPressed");

        // Only trigger on left-click
        if (!e.GetCurrentPoint((Visual)sender!).Properties.IsLeftButtonPressed) return;
        if (sender is not Canvas { ContextMenu: { } menu } canvas) return;
        menu.PlacementTarget = canvas;
        menu.Placement = PlacementMode.RightEdgeAlignedTop; // Menu-style position
        menu.Open();
    }

    private void ToggleThemeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        App.Logger!.LogTrace("ToggleThemeItem_OnClick");

        RequestedThemeVariant = _debounce ? ThemeVariant.Light : ThemeVariant.Dark;
        _debounce = !_debounce;
    }

    private void MenuBar_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        MenuBar.Width += 100;
    }

    private void MenuBar_OnPointerExited(object? sender, PointerEventArgs e)
    {
        MenuBar.Width -= 100;
    }

    private void HomeItem_OnClick(object? sender, RoutedEventArgs e)
    {
        App.Logger!.LogTrace("HomeItem_OnClick");
        _navigationService.Navigate<HomeView>();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _navigationService.Navigate<PlaylistView>();
    }
}
using System;
using Avalonia.Controls;
using Meio.desktop.Interfaces.Services;
using Meio.desktop.Services;

namespace Meio.desktop.Views;

public partial class HomeView : UserControl, INavigable
{
    private readonly INavigationService _navigationService;

    public HomeView(INavigationService navigationService)
    {
        InitializeComponent();
        _navigationService = navigationService;
    }

    public void OnNavigateTo(object? parameter)
    {
        Console.WriteLine("meow");
    }
}
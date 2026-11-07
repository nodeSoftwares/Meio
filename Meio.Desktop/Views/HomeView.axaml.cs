using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media;
using Meio.desktop.Interfaces.Services;
using Meio.desktop.Models;
using Meio.desktop.Services;

namespace Meio.desktop.Views;

public partial class HomeView : UserControl, INavigable
{
    private readonly INavigationService _navigationService;

    // For the avalonia previewer
    public HomeView()
    {
        InitializeComponent();
        DataContext = this;
    }

    public HomeView(INavigationService navigationService)
    {
        InitializeComponent();
        DataContext = this;
        _navigationService = navigationService;
    }

    public ObservableCollection<CardModel> DummyCardsList { get; set; } =
    [
        new("Nirvana", new SolidColorBrush(Colors.IndianRed)),
        new("Get Away", new SolidColorBrush(Colors.CornflowerBlue)),
        new("Only love for myself", new SolidColorBrush(Colors.YellowGreen)),
        new("please revere me, my purpose eludes me", new SolidColorBrush(Colors.DarkSlateBlue)),
        new("Don't you run from me", new SolidColorBrush(Colors.Yellow)),
        new("Koï", new SolidColorBrush(Colors.Orange)),
        new("novulent", new SolidColorBrush(Colors.Green)),
        new("TACTIC", new SolidColorBrush(Colors.DarkGoldenrod)),
        new("505", new SolidColorBrush(Colors.LightPink)),
        new("Numb", new SolidColorBrush(Colors.DarkSeaGreen))
    ];

    public void OnNavigateTo(object? parameter)
    {
        Console.WriteLine("meow");
    }
}
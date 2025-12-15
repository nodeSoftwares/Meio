using Avalonia.Controls;
using Meio.app.Services;
using Microsoft.Extensions.Logging;

namespace Meio.app.Views;

public partial class HomeView : UserControl, INavigable
{
    public HomeView()
    {
        InitializeComponent();
    }


    public void OnNavigateTo(object? parameter)
    {
        App.Logger!.LogTrace("Navigated to the home page.");
    }
}
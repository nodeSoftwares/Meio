using System;
using Avalonia.Controls;
using Meio.desktop.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Meio.desktop.Services;

public interface INavigable
{
    void OnNavigateTo(object? parameter);
}

public class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private Action<UserControl>? _navigateAction;

    public void SetNavigator(Action<UserControl> navigateAction)
    {
        _navigateAction = navigateAction;
    }

    public void Navigate<T>(object? parameter = null) where T : UserControl
    {
        var page = serviceProvider.GetRequiredService<T>();
        if (page is INavigable navigable)
        {
            navigable.OnNavigateTo(parameter);
        }

        _navigateAction!(page);
    }
}
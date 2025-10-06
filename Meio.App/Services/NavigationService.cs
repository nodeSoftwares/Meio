using System;
using Avalonia.Controls;

namespace Meio.app.Services;

public interface INavigable
{
    void OnNavigateTo(object? parameter);
}

public class NavigationService(Action<UserControl> navigateAction)
{
    public void Navigate<T>(object? parameter = null) where T : UserControl, new()
    {
        var page = new T();
        if (page is INavigable navigable)
        {
            navigable.OnNavigateTo(parameter);
        }

        navigateAction(page);
    }
}
using System;
using Avalonia.Controls;

namespace Meio.desktop.Interfaces.Services;

public interface INavigationService
{
    public void Navigate<T>(object? parameter = null) where T : UserControl;

    public void SetNavigator(Action<UserControl> navigateAction);
}
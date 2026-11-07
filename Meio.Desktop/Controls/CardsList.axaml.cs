using System.Collections;
using Avalonia;
using Avalonia.Controls;

namespace Meio.desktop.Controls;

public partial class CardsList : UserControl
{
    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.Register<CardsList, IEnumerable>(nameof(ItemsSource));

    public CardsList()
    {
        InitializeComponent();
    }

    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
}
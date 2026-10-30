using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Meio.desktop.Controls;

public partial class Card : UserControl
{
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<Card, string>(nameof(Title));

    public static readonly StyledProperty<Brush> CoverBrushProperty = AvaloniaProperty.Register<Card, Brush>(nameof(CoverBrush));

    public Card()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public Brush CoverBrush
    {
        get => GetValue(CoverBrushProperty);
        set => SetValue(CoverBrushProperty, value);
    }
}
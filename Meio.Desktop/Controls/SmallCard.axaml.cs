using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Meio.desktop.Controls;

public partial class SmallCard : UserControl
{
    public static readonly StyledProperty<Brush> CoverBrushProperty = AvaloniaProperty.Register<SmallCard, Brush>("CoverBrush");

    public SmallCard()
    {
        InitializeComponent();
    }

    public Brush CoverBrush
    {
        get => GetValue(CoverBrushProperty);
        set => SetValue(CoverBrushProperty, value);
    }
}
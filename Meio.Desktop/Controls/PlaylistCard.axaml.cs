using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Meio.desktop.Controls;

public partial class PlaylistCard : UserControl
{
    public static readonly StyledProperty<Brush> CoverBrushProperty = AvaloniaProperty.Register<PlaylistCard, Brush>("CoverBrush");

    public PlaylistCard()
    {
        InitializeComponent();
    }

    public Brush CoverBrush
    {
        get => GetValue(CoverBrushProperty);
        set => SetValue(CoverBrushProperty, value);
    }
}
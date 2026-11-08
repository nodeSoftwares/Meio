using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Meio.desktop.Controls;

public partial class BackgroundCircle : UserControl
{
    // public static readonly StyledProperty<double> RandomWidthProperty =
    //     AvaloniaProperty.Register<BackgroundCircle, double>(nameof(RandomWidth));
    //
    // public static readonly StyledProperty<double> RandomHeightProperty =
    //     AvaloniaProperty.Register<BackgroundCircle, double>(nameof(RandomHeight));

    public static readonly StyledProperty<Brush> ColorProperty =
        AvaloniaProperty.Register<BackgroundCircle, Brush>(nameof(Color));

    public BackgroundCircle()
    {
        InitializeComponent();
    }

    // public double RandomWidth
    // {
    //     get => GetValue(RandomWidthProperty);
    //     set => SetValue(RandomWidthProperty, value);
    // }
    //
    // public double RandomHeight
    // {
    //     get => GetValue(RandomHeightProperty);
    //     set => SetValue(RandomHeightProperty, value);
    // }

    public Brush Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
}
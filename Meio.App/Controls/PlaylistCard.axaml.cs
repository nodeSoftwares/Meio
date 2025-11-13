using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace Meio.app.Controls;

public class PlaylistCard : TemplatedControl
{
    public static readonly StyledProperty<IBrush?> CoverBrushProperty =
        AvaloniaProperty.Register<PlaylistCard, IBrush?>(nameof(CoverBrush));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<PlaylistCard, ICommand?>(nameof(Command));

    public IBrush? CoverBrush
    {
        get => GetValue(CoverBrushProperty);
        set => SetValue(CoverBrushProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (Command?.CanExecute(null) == true)
            Command.Execute(null);
    }
}
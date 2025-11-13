using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace Meio.app.Controls;

public class Card : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<Card, string>(nameof(Title));

    public static readonly StyledProperty<IBrush?> CoverBrushProperty =
        AvaloniaProperty.Register<Card, IBrush?>(nameof(CoverBrush));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<Card, ICommand?>(nameof(Command));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

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
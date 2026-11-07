using Avalonia.Media;

namespace Meio.desktop.Models;

public class CardModel(string title, Brush coverBrush)
{
    public string Title { get; set; } = title;

    public Brush CoverBrush { get; set; } = coverBrush;
}
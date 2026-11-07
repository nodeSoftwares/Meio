using Avalonia.Media;

namespace Meio.desktop.Models;

public class PlaylistCardModel(Brush coverBrush)
{
    public Brush CoverBrush { get; set; } = coverBrush;
}
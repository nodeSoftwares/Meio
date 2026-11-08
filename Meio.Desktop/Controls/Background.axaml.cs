using System;
using Avalonia.Controls;
using Avalonia.Media;
using Meio.desktop.Helpers;

namespace Meio.desktop.Controls;

public partial class Background : UserControl
{
    public Background()
    {
        InitializeComponent();

        var random = new Random();
        var count = 10;

        const double maxX = 1366;
        const double maxY = 768;

        const double maxHeight = maxY / 2;

        for (var i = 0; i < count; i++)
        {
            var randomX = random.NextDouble() * maxX;
            var randomY = random.NextDouble() * maxY;

            var randomSize = random.NextDouble() * maxHeight;

            var backgroundCircle = new BackgroundCircle
            {
                Width = randomSize,
                Height = randomSize,
                Color = new SolidColorBrush(ColorHelper.GetRandomColor())
            };

            Canvas.SetLeft(backgroundCircle, randomX);
            Canvas.SetTop(backgroundCircle, randomY);

            BackgroundCanvas.Children.Add(backgroundCircle);
        }
    }
}
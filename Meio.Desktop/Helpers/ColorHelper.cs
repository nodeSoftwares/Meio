using System;
using Avalonia.Media;

namespace Meio.desktop.Helpers;

public static class ColorHelper
{
    /// <summary>
    ///     Get a random color.
    /// </summary>
    /// <returns>A random color.</returns>
    public static Color GetRandomColor()
    {
        var random = new Random();

        var r = (byte)random.Next(0, 256);
        var g = (byte)random.Next(0, 256);
        var b = (byte)random.Next(0, 256);

        return Color.FromRgb(r, g, b);
    }

    /// <summary>
    ///     Get a random color.
    /// </summary>
    /// <param name="rnd">Use an existing Random instance to generate the color.</param>
    /// <returns></returns>
    public static Color GetRandomColor(Random rnd)
    {
        var r = (byte)rnd.Next(0, 256);
        var g = (byte)rnd.Next(0, 256);
        var b = (byte)rnd.Next(0, 256);

        return Color.FromRgb(r, g, b);
    }
}
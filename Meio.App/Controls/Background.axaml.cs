using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;

namespace Meio.app.Controls;

public class Background : TemplatedControl
{
    private new const int Width = 1366;
    private new const int Height = 768;
    private new const int MinEllipseSize = 50;
    private static readonly Ellipse[] Shapes = [];
    private Canvas? _canvas;
    private static readonly Random Random = new(); // TODO: Better randomisation

    // TODO: variable ellipse count and brushes via settings
    // TODO: Convenient ellipse count according to window size
    private const int Count = 20;

    private readonly IBrush[] _brushes =
    [
        new SolidColorBrush(Color.Parse("#BFA83939")),
        new SolidColorBrush(Color.Parse("#BFFFCACA")),
        new SolidColorBrush(Color.Parse("#BF772727"))
    ];

    private static void GenerateEllipses(int count, IBrush[] brushes, Canvas canvas)
    {
        for (var i = 0; i < count; i++)
        {
            var posX = Random.NextDouble() * Width;
            var posY = Random.NextDouble() * Height;
            var size = Random.Next(MinEllipseSize, Height / 2);
            var color = brushes[Random.Next(0, brushes.Length)];

            var ellipse = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = color,
                ZIndex = 0
            };

            Shapes.Append(ellipse);

            ellipse.SetValue(Canvas.LeftProperty, posX);
            ellipse.SetValue(Canvas.TopProperty, posY);

            canvas.Children.Add(ellipse);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        // e.NameScope.FindResource("PrimaryBrush") as IBrush;
        base.OnApplyTemplate(e);
        _canvas = e.NameScope.Get<Canvas>("Background");
        _canvas.PointerMoved += (s, e) => Console.WriteLine("Pointer moved!"); // TODO: not working car pas focus?

        GenerateEllipses(Count, _brushes, _canvas);
    }

    /*private static double[] GenerateMoveSpeeds()
    {
        var speeds = new double[Shapes.Length];

        for (var i = 0; i < Shapes.Length; i++)
        {
            speeds.Append(Random.NextDouble());
        }

        return speeds;
    }

    private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_canvas == null) return;

        var speeds = GenerateMoveSpeeds();
        Console.WriteLine(speeds);

        var pos = e.GetPosition(this);
        var centerX = _canvas.Bounds.Width / 2;
        var centerY = _canvas.Bounds.Height / 2;

        var offsetX = pos.X - centerX;
        var offsetY = pos.Y - centerY;

        for (var i = 0; i < Shapes.Length; i++)
        {
            Shapes[i].SetValue(Canvas.LeftProperty, centerX + offsetX * speeds[i]);
            Shapes[i].SetValue(Canvas.TopProperty, centerY + offsetY * speeds[i]);
        }
    }*/
}
using System.IO;
using Avalonia.Media.Imaging;

namespace Meio.app.Services;

public class ImageHelper
{
    public static Bitmap? LoadBitmapFromBytes(byte[] imageData)
    {
        if (imageData.Length == 0) return null;

        using var stream = new MemoryStream(imageData);
        return new Bitmap(stream);
    }
}
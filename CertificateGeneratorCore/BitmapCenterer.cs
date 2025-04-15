using System.Drawing;
using SkiaSharp;

namespace CertificateGeneratorCore;

public class BitmapCenterer
{
    private readonly Rectangle _bounds;

    public BitmapCenterer(Rectangle bounds)
    {
        _bounds = bounds;
    }

    public Rectangle Center(SKBitmap bitmap)
    {
        // Resize bitmap to fit within the bounds but maintain aspect ratio.
        float scaleX = (float)_bounds.Width / bitmap.Width;
        float scaleY = (float)_bounds.Height / bitmap.Height;
        float scale = Math.Min(scaleX, scaleY);
        int newWidth = (int)(bitmap.Width * scale);
        int newHeight = (int)(bitmap.Height * scale);
        int x = _bounds.X + (_bounds.Width - newWidth) / 2;
        int y = _bounds.Y + (_bounds.Height - newHeight) / 2;
        return new Rectangle(x, y, newWidth, newHeight);
    }
}
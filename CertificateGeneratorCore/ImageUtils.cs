using SkiaSharp;

namespace CertificateGeneratorCore;

public static class ImageUtils
{
    public static SKBitmap CreateBitmap(int width, int height, SKColor color)
    {
        var bitmap = new SKBitmap(width, height);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(color);
        }

        return bitmap;
    }

    public static SKBitmap CreateBlackBitmap()
    {
        return CreateBlackBitmap(3507, 2480);
    }

    public static SKBitmap CreateBlackBitmap(int width, int height)
    {
        return CreateBitmap(width, height, SKColors.Black);
    }
}
using SkiaSharp;

namespace CertificateGeneratorCore;

public static class ImageUtils
{
    public static SKBitmap CreateBlackBitmap()
    {
        var bitmap = new SKBitmap(3507, 2480);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors.Black);
        }

        return bitmap;
    }

    public static MemoryStream CreateBlackTemplate()
    {
        using var bitmap = new SKBitmap(3507, 2480);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors.Black);
        }

        using var image = SKImage.FromBitmap(bitmap);
        var data = image.Encode(SKEncodedImageFormat.Png, 100);

        // Create a memory stream
        var memoryStream = new MemoryStream();
        data.SaveTo(memoryStream);

        // Use the memory stream as needed
        // For example, reset the position to the beginning
        memoryStream.Position = 0;

        return memoryStream;
    }
}
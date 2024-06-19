using SkiaSharp;

namespace AdopteerRegenwoud.CertificateGeneratorCore;

public class CertificateGenerator(Stream certificateTemplate)
{
    private readonly Stream _certificateTemplate = certificateTemplate;

    public Stream Generate(AdoptionRecord adoptionRecord)
    {
        using var inputStream = new SKManagedStream(_certificateTemplate);
        using var bitmap = SKBitmap.Decode(inputStream);
        using var canvas = new SKCanvas(bitmap);
        var font = SKTypeface.FromFamilyName("Arial");

        var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = 40,
            IsAntialias = true,
            Typeface = font
        };

        var point = new SKPoint(100, 100);
        canvas.DrawText($"Name: {adoptionRecord.Name}", point, paint);

        point.Y += 50;
        canvas.DrawText($"Square Meters: {adoptionRecord.SquareMeters}", point, paint);

        point.Y += 50;
        canvas.DrawText($"Date: {adoptionRecord.Date}", point, paint);

        point.Y += 50;
        canvas.DrawText($"Language: {adoptionRecord.Language}", point, paint);

        SKData outputImageData = CreatePngFromBitmap(bitmap);
        return CreateMemoryStreamFromImageData(outputImageData);
    }

    private static SKData CreatePngFromBitmap(SKBitmap bitmap)
    {
        using var image = SKImage.FromBitmap(bitmap);
        return image.Encode(SKEncodedImageFormat.Png, 100);
    }

    private static MemoryStream CreateMemoryStreamFromImageData(SKData imageData)
    {
        var outputStream = new MemoryStream();
        imageData.SaveTo(outputStream);
        // Reset the stream position to the beginning so calling code can directly start reading.
        outputStream.Position = 0;
        return outputStream;
    }
}

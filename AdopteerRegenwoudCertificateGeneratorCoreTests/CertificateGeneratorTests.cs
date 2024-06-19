using SkiaSharp;

namespace AdopteerRegenwoudCertificateGeneratorCoreTests;

public class CertificateGeneratorTests
{
    [Test]
    public void TestGenerate()
    {
        // Arrange
        var adoptionRecord = new AdoptionRecord("Janssen", 20, new DateOnly(2024, 6, 19), Language.Dutch);
        Stream templateStream = CreateBlackTemplate();
        var sut = new CertificateGenerator(templateStream);

        // Act
        Stream generatedStream = sut.Generate(adoptionRecord);

        // Assert
        Assert.That(AreAllPixelsBlack(generatedStream), Is.False);
    }

    private static bool AreAllPixelsBlack(Stream memoryStream)
    {
        memoryStream.Position = 0;
        using var managedStream = new SKManagedStream(memoryStream);
        using var bitmap = SKBitmap.Decode(managedStream);

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                if (color != SKColors.Black)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static MemoryStream CreateBlackTemplate()
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

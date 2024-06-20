using SkiaSharp;

namespace CertificateGeneratorCoreTests;

public class CertificateGeneratorTests
{
    [Test]
    public void TestGenerateOne()
    {
        // Arrange
        var adoptionRecord = new AdoptionRecord("Janssen", 20, new DateOnly(2024, 6, 19), Language.Dutch);
        using Stream templateStream = ImageUtils.CreateBlackTemplate();
        var sut = new CertificateGenerator(templateStream);

        // Act
        Stream generatedStream = sut.Generate(adoptionRecord);

        // Assert
        Assert.That(AreAllPixelsBlack(generatedStream), Is.False);
    }

    [Test]
    public void TestGenerateTwo()
    {
        // Arrange
        var adoptionRecord = new AdoptionRecord("Janssen", 20, new DateOnly(2024, 6, 19), Language.Dutch);
        using Stream templateStream = ImageUtils.CreateBlackTemplate();
        var sut = new CertificateGenerator(templateStream);
        Stream generatedStream = sut.Generate(adoptionRecord);

        // Act
        generatedStream = sut.Generate(adoptionRecord);

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
}

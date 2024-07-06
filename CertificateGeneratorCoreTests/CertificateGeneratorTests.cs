using SkiaSharp;

namespace CertificateGeneratorCoreTests;

public class CertificateGeneratorTests
{

    [Test]
    public void TestGenerateOne()
    {
        // Arrange
        var adoptionRecord = new AdoptionRecord("Janssen", 20, new DateOnly(2024, 6, 19), Language.Dutch);
        var sut = new CertificateGenerator(new DummyTemplateBitmapReceiver());

        // Act
        CertificateGenerator.Result result = sut.Generate(adoptionRecord);

        // Assert
        Assert.That(AreAllPixelsBlack(result.Jpg3MbStream), Is.False);
    }

    [Test]
    public void TestGenerateTwo()
    {
        // Arrange
        var adoptionRecord = new AdoptionRecord("Janssen", 20, new DateOnly(2024, 6, 19), Language.Dutch);
        using Stream templateStream = ImageUtils.CreateBlackTemplate();
        var sut = new CertificateGenerator(new DummyTemplateBitmapReceiver());
        CertificateGenerator.Result result = sut.Generate(adoptionRecord);

        // Act
        result = sut.Generate(adoptionRecord);

        // Assert
        Assert.That(AreAllPixelsBlack(result.Jpg3MbStream), Is.False);
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

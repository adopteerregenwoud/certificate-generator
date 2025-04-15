using System.Drawing;
using SkiaSharp;

namespace CertificateGeneratorCoreTests;

public class CertificateGeneratorTests
{

    [Test]
    public void TestGenerateOne()
    {
        // Arrange
        var adoptionRecord = new AdoptionRecord("Janssen", 20, "19-06-2024", Language.Dutch);
        var sut = new CertificateGenerator(new DummyBitmapRetriever(), CertificateTemplateConfig.Default);

        // Act
        CertificateGenerator.Result result = sut.GenerateJpg(adoptionRecord);

        // Assert
        Assert.That(AreAllPixelsBlack(result.Jpg3MbStream), Is.False);
    }

    [Test]
    public void TestGenerateTwo()
    {
        // Arrange
        var adoptionRecord = new AdoptionRecord("Janssen", 20, "19-06-2024", Language.Dutch);
        var sut = new CertificateGenerator(new DummyBitmapRetriever(), CertificateTemplateConfig.Default);

        // Act
        sut.GenerateJpg(adoptionRecord);
        CertificateGenerator.Result result = sut.GenerateJpg(adoptionRecord);

        // Assert
        Assert.That(AreAllPixelsBlack(result.Jpg3MbStream), Is.False);
    }

    [Test]
    public void GenerateBitmap_ShouldRenderLogo_WhenThereIsALogoBitmap()
    {
        // Arrange
        var adoptionRecord = new AdoptionRecord("Janssen", 20, "19-06-2024", Language.Dutch);
        CertificateTemplateConfig config = CertificateTemplateConfig.Default;
        config.LogoBoundingBox = new Rectangle(50, 50, 100, 100);
        var sut = new CertificateGenerator(new DummyBitmapRetriever(), config);

        // Act
        SKBitmap result = sut.GenerateBitmap(adoptionRecord);

        // Assert
        Assert.That(result.GetPixel(75, 75), Is.EqualTo(SKColors.Red));
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

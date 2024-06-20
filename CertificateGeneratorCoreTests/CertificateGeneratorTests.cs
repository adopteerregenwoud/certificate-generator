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
        CertificateGenerator.Result result = sut.Generate(adoptionRecord);

        // Assert
        Assert.That(AreAllPixelsBlack(result.FullSizePngStream), Is.False);
        Assert.That(AreAllPixelsBlack(result.Jpg3MbStream), Is.False);
    }

    [Test]
    public void TestGenerateTwo()
    {
        // Arrange
        var adoptionRecord = new AdoptionRecord("Janssen", 20, new DateOnly(2024, 6, 19), Language.Dutch);
        using Stream templateStream = ImageUtils.CreateBlackTemplate();
        var sut = new CertificateGenerator(templateStream);
        CertificateGenerator.Result result = sut.Generate(adoptionRecord);

        // Act
        result = sut.Generate(adoptionRecord);

        // Assert
        Assert.That(AreAllPixelsBlack(result.FullSizePngStream), Is.False);
        Assert.That(AreAllPixelsBlack(result.Jpg3MbStream), Is.False);
    }

    [TestCase("hello", "hello", null)]
    [TestCase("hello world", "hello world", null)]
    [TestCase("Jane Smith en ook nog vele anderen", "Jane Smith en ook", "nog vele anderen")]
    public void TestWrapText(string original, string firstWrappedLine, string? secondWrappedLine)
    {
        // Arrange
        using Stream templateStream = ImageUtils.CreateBlackTemplate();
        var sut = new CertificateGenerator(templateStream);
        var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = CertificateGenerator.FontSizeName,
            IsAntialias = true,
            Typeface = sut.RobotoSlabTypefaceMedium
        };

        // Act
        IList<string> actual = CertificateGenerator.WrapText(original, paint, CertificateGenerator.MaxNameWidth);

        // Assert
        Assert.That(actual.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(actual.Count, Is.LessThanOrEqualTo(2));
        Assert.That(actual[0], Is.EqualTo(firstWrappedLine));
        if (actual.Count > 1)
        {
            Assert.That(actual[1], Is.EqualTo(secondWrappedLine));
        }
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

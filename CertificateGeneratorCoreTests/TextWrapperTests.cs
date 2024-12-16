using SkiaSharp;

namespace CertificateGeneratorCoreTests;

public class TextWrapperTests
{
    [TestCase("hello", "hello", null)]
    [TestCase("hello world", "hello world", null)]
    [TestCase("Jane Smith en ook nog vele anderen", "Jane Smith en ook", "nog vele anderen")]
    public void TestWrapText(string original, string firstWrappedLine, string? secondWrappedLine)
    {
        // Arrange
        using Stream templateStream = ImageUtils.CreateBlackTemplate();
        var sut = new CertificateGenerator(new DummyTemplateBitmapReceiver(), CertificateTemplateConfig.Default);
        var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = sut.Config.NameFontSize,
            IsAntialias = true,
            Typeface = sut.RobotoSlabTypefaceMedium
        };

        // Act
        IList<string> actual = TextWrapper.WrapText(original, paint, sut.Config.NameMaxWidth);

        // Assert
        Assert.That(actual.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(actual.Count, Is.LessThanOrEqualTo(2));
        Assert.That(actual[0], Is.EqualTo(firstWrappedLine));
        if (actual.Count > 1)
        {
            Assert.That(actual[1], Is.EqualTo(secondWrappedLine));
        }
    }
}
using System.Drawing;
using SkiaSharp;

namespace CertificateGeneratorCoreTests;

public class BitmapCentererTests
{
    [Test]
    public void Center_ShouldReturnBoundingBox_WhenBitmapEqualsBounds()
    {
        // Arrange
        var bounds = new Rectangle(100, 100, 100, 100);
        SKBitmap bitmap = ImageUtils.CreateBlackBitmap(width: 100, height: 100);
        var centerer = new BitmapCenterer(bounds);

        // Act
        Rectangle actual = centerer.Center(bitmap);

        // Assert
        Rectangle expected = new(100, 100, 100, 100);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Center_ShouldReturnBoundingBox_WhenBitmapHasSameShapeButSmaller()
    {
        // Arrange
        var bounds = new Rectangle(100, 100, 100, 100);
        SKBitmap bitmap = ImageUtils.CreateBlackBitmap(width: 50, height: 50);
        var centerer = new BitmapCenterer(bounds);

        // Act
        Rectangle actual = centerer.Center(bitmap);

        // Assert
        Rectangle expected = new(100, 100, 100, 100);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Center_ShouldReturnBoundingBox_WhenBitmapHasSameShapeButLarger()
    {
        // Arrange
        var bounds = new Rectangle(100, 100, 100, 100);
        SKBitmap bitmap = ImageUtils.CreateBlackBitmap(width: 200, height: 200);
        var centerer = new BitmapCenterer(bounds);

        // Act
        Rectangle actual = centerer.Center(bitmap);

        // Assert
        Rectangle expected = new(100, 100, 100, 100);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Center_ShouldReturnFullWidth_WhenBitmapIsFlatterAndSmaller()
    {
        // Arrange
        var bounds = new Rectangle(100, 100, 100, 100);
        SKBitmap bitmap = ImageUtils.CreateBlackBitmap(width: 50, height: 25);
        var centerer = new BitmapCenterer(bounds);

        // Act
        Rectangle actual = centerer.Center(bitmap);

        // Assert
        Rectangle expected = new(100, 125, 100, 50);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Center_ShouldReturnFullWidth_WhenBitmapIsPortraitAndLarger()
    {
        // Arrange
        var bounds = new Rectangle(100, 100, 100, 100);
        SKBitmap bitmap = ImageUtils.CreateBlackBitmap(width: 200, height: 100);
        var centerer = new BitmapCenterer(bounds);

        // Act
        Rectangle actual = centerer.Center(bitmap);

        // Assert
        Rectangle expected = new(100, 125, 100, 50);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Center_ShouldReturnFullHeight_WhenBitmapIsPortraitAndSmaller()
    {
        // Arrange
        var bounds = new Rectangle(100, 100, 100, 100);
        SKBitmap bitmap = ImageUtils.CreateBlackBitmap(width: 25, height: 50);
        var centerer = new BitmapCenterer(bounds);

        // Act
        Rectangle actual = centerer.Center(bitmap);

        // Assert
        Rectangle expected = new(125, 100, 50, 100);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Center_ShouldReturnFullHeight_WhenBitmapIsPortraitAndLarger()
    {
        // Arrange
        var bounds = new Rectangle(100, 100, 100, 100);
        SKBitmap bitmap = ImageUtils.CreateBlackBitmap(width: 100, height: 200);
        var centerer = new BitmapCenterer(bounds);

        // Act
        Rectangle actual = centerer.Center(bitmap);

        // Assert
        Rectangle expected = new(125, 100, 50, 100);
        Assert.That(actual, Is.EqualTo(expected));
    }
}


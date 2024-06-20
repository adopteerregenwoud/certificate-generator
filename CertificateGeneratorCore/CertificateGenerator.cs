using System.Reflection;
using SkiaSharp;

namespace CertificateGeneratorCore;

public class CertificateGenerator
{
    private readonly SKBitmap _certificateTemplateBitmap;
    private readonly SKTypeface _robotoSlabTypeface;

    private const int rightMarginSquareMeters = 60;
    private const int topMarginSquareMeters = 40;
    private const int fontSizeSquareMeters = 330;
    private const int leftMarginName = 810;
    private const int bottomMarginName = 750;
    private const int fontSizeName = 175;
    private const int leftMarginDate = leftMarginName;
    private const int bottomMarginDate = 460;
    private const int fontSizeDate = 50;

    public CertificateGenerator(Stream certificateTemplateStream)
    {
        _certificateTemplateBitmap = ReadBitmapFromStream(certificateTemplateStream);
        _robotoSlabTypeface = ReadFontFromEmbeddedResource();
    }

    private static SKBitmap ReadBitmapFromStream(Stream certificateTemplateStream)
    {
        using var inputStream = new SKManagedStream(certificateTemplateStream);
        return SKBitmap.Decode(inputStream);
    }

    private static SKTypeface ReadFontFromEmbeddedResource()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "CertificateGeneratorCore.fonts.RobotoSlab-VariableFont_wght.ttf";

        using Stream? fontStream = assembly.GetManifestResourceStream(resourceName);
        if (fontStream == null)
        {
            throw new FileNotFoundException("Font file not found in embedded resources.");
        }

        return SKTypeface.FromStream(fontStream);
    }

    public Stream Generate(AdoptionRecord adoptionRecord)
    {
        using var bitmap = _certificateTemplateBitmap.Copy();
        using var canvas = new SKCanvas(bitmap);

        RenderSquareMeters(canvas, bitmap, adoptionRecord.SquareMeters);
        RenderName(canvas, bitmap, adoptionRecord.Name);
        RenderDate(canvas, bitmap, adoptionRecord.Date);

        SKData outputImageData = CreatePngFromBitmap(bitmap);
        return CreateMemoryStreamFromImageData(outputImageData);
    }

    private void RenderSquareMeters(SKCanvas canvas, SKBitmap bitmap, int squareMeters)
    {
        var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = fontSizeSquareMeters,
            IsAntialias = true,
            Typeface = _robotoSlabTypeface
        };

        string text = $"{squareMeters}m²";
        float textSize = paint.MeasureText(text);

        var point = new SKPoint(bitmap.Width - rightMarginSquareMeters - textSize, topMarginSquareMeters + fontSizeSquareMeters);
        canvas.DrawText(text, point, paint);
    }

    private void RenderName(SKCanvas canvas, SKBitmap bitmap, string name)
    {
        var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = fontSizeName,
            IsAntialias = true,
            Typeface = _robotoSlabTypeface
        };

        var point = new SKPoint(leftMarginName, bitmap.Height - bottomMarginName);
        canvas.DrawText(name, point, paint);
    }

    private void RenderDate(SKCanvas canvas, SKBitmap bitmap, DateOnly date)
    {
        var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = fontSizeDate,
            IsAntialias = true,
            Typeface = _robotoSlabTypeface
        };

        string text = $"{date:dd-MM-yyy}";

        var point = new SKPoint(leftMarginDate, bitmap.Height - bottomMarginDate);
        canvas.DrawText(text, point, paint);
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

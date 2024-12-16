using System.Reflection;
using SkiaSharp;

namespace CertificateGeneratorCore;

public class CertificateGenerator
{
    public class Result
    {
        public required Stream Jpg3MbStream { get; set; }
    }

    private readonly ITemplateBitmapRetriever _templateBitmapRetriever;
    public SKTypeface RobotoSlabTypefaceMedium { get; private set; }
    public SKTypeface RobotoSlabTypefaceRegular { get; private set; }
    private readonly CertificateTemplateConfig _config;

    public const int MaxNameWidth = 1670;
    public const int FontSizeName = 175;
    private const int leftMarginDate = 810;
    private const int bottomMarginDate = 460;
    private const int fontSizeDate = 50;
    private const int jpgQuality = 94;

    public CertificateGenerator(ITemplateBitmapRetriever templateBitmapRetriever)
    {
        _templateBitmapRetriever = templateBitmapRetriever;

        RobotoSlabTypefaceMedium = ReadFontFromEmbeddedResource("CertificateGeneratorCore.fonts.RobotoSlab-Medium.ttf");
        RobotoSlabTypefaceRegular = ReadFontFromEmbeddedResource("CertificateGeneratorCore.fonts.RobotoSlab-Regular.ttf");

        _config = CertificateTemplateConfig.Default;
    }

    private static SKTypeface ReadFontFromEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using Stream? fontStream = assembly.GetManifestResourceStream(resourceName);
        if (fontStream == null)
        {
            throw new FileNotFoundException("Font file not found in embedded resources.");
        }

        return SKTypeface.FromStream(fontStream);
    }

    public Result Generate(AdoptionRecord adoptionRecord)
    {
        using var bitmap = _templateBitmapRetriever.Retrieve(adoptionRecord.SquareMeters, adoptionRecord.Language).Copy();
        using var canvas = new SKCanvas(bitmap);

        RenderSquareMeters(canvas, bitmap, adoptionRecord.SquareMeters);
        RenderName(canvas, bitmap, adoptionRecord.Name);
        RenderDate(canvas, bitmap, adoptionRecord.Date);

        SKData imageDataJpg = CreateJpgFromBitmap(bitmap);

        return new Result
        {
            Jpg3MbStream = CreateMemoryStreamFromImageData(imageDataJpg)
        };
    }

    private void RenderSquareMeters(SKCanvas canvas, SKBitmap bitmap, int squareMeters)
    {
        const int dropShadowDelta = 15;
        const int dropShadowSigma = 15;

        CertificateTemplateType templateType = CertificateTemplateTypeHelper.GetTypeFromAreaSize(squareMeters);
        SKColor textColor = ConvertToSkColor(_config[templateType].AreaColor);
        int fontSize = _config[templateType].AreaFontSize;

        // We don't want the size of dropshadow to influence the location of the text.
        // So we measure with a different paint than we actually draw.
        var paintForMeasure = new SKPaint
        {
            Color = textColor,
            TextSize = fontSize,
            IsAntialias = true,
            Typeface = RobotoSlabTypefaceRegular
        };
        var paintForRender = new SKPaint
        {
            Color = textColor,
            TextSize = fontSize,
            IsAntialias = true,
            Typeface = RobotoSlabTypefaceRegular,
            ImageFilter = SKImageFilter.CreateDropShadow(
                dropShadowDelta, dropShadowDelta,
                dropShadowSigma, dropShadowSigma,
                new SKColor(0, 0, 0, 128))
        };

        string m2Text = $"m²";
        float m2TextSize = paintForMeasure.MeasureText(m2Text);
        var point = new SKPoint(bitmap.Width - _config.AreaRightMargin - m2TextSize, _config.AreaTopMargin + fontSize);
        canvas.DrawText(m2Text, point, paintForRender);

        paintForMeasure.Typeface = RobotoSlabTypefaceMedium;
        paintForRender.Typeface = RobotoSlabTypefaceMedium;
        string text = $"{squareMeters}";
        float textSize = paintForMeasure.MeasureText(text);

        point = new SKPoint(bitmap.Width - _config.AreaRightMargin - m2TextSize - textSize, _config.AreaTopMargin + fontSize);
        canvas.DrawText(text, point, paintForRender);
    }

    private SKColor ConvertToSkColor(RgbColor areaColor)
    {
        return new SKColor((byte)areaColor.R, (byte)areaColor.G, (byte)areaColor.B, 255);
    }

    private void RenderName(SKCanvas canvas, SKBitmap bitmap, string name)
    {
        var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = FontSizeName,
            IsAntialias = true,
            Typeface = RobotoSlabTypefaceMedium
        };

        IList<string> wrappedLines = TextWrapper.WrapText(name, paint, MaxNameWidth);

        int firstLineYOffset = 0;
        if (wrappedLines.Count == 1)
        {
            firstLineYOffset = FontSizeName / 2;
        }
        var point = new SKPoint(_config.NameLeftMargin, bitmap.Height - _config.NameBottomMargin + firstLineYOffset);
        canvas.DrawText(wrappedLines[0], point, paint);

        if (wrappedLines.Count > 1)
        {
            point = new SKPoint(_config.NameLeftMargin, bitmap.Height - _config.NameBottomMargin + FontSizeName);
            canvas.DrawText(wrappedLines[1], point, paint);
        }

        if (wrappedLines.Count > 2)
        {
            throw new NotImplementedException("No code for very long lines yet.");
        }
    }

    private void RenderDate(SKCanvas canvas, SKBitmap bitmap, string date)
    {
        var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = fontSizeDate,
            IsAntialias = true,
            Typeface = RobotoSlabTypefaceRegular
        };

        var point = new SKPoint(leftMarginDate, bitmap.Height - bottomMarginDate);
        canvas.DrawText(date, point, paint);
    }

    private static SKData CreateJpgFromBitmap(SKBitmap bitmap)
    {
        using var image = SKImage.FromBitmap(bitmap);
        return image.Encode(SKEncodedImageFormat.Jpeg, jpgQuality);
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

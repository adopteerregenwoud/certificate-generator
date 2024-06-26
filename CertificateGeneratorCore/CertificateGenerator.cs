using System.Reflection;
using SkiaSharp;

namespace CertificateGeneratorCore;

public class CertificateGenerator
{
    public class Result
    {
        public required Stream FullSizePngStream { get; set; }
        public required Stream Jpg3MbStream { get; set; }
    }

    private readonly ITemplateBitmapRetriever _templateBitmapRetriever;
    public SKTypeface RobotoSlabTypefaceMedium { get; private set; }
    private readonly SKTypeface _robotoSlabTypefaceRegular;

    private readonly Dictionary<int, SKColor> _areaColors = new()
    {
        [1] = new SKColor(196, 217, 117, 255),
        [4] = SKColors.White,
        [10] = new SKColor(196, 217, 117, 255),
        [20] = SKColors.White,
        [50] = new SKColor(196, 217, 117, 255),
        [100] = new SKColor(123, 103, 91, 255)
    };
    private readonly Dictionary<int, int> _areaFontsizes = new()
    {
        [1] = 390,
        [4] = 430,
        [10] = 430,
        [20] = 360,
        [50] = 430,
        [100] = 430
    };

    private const int rightMarginSquareMeters = 60;
    private const int topMarginSquareMeters = 0;
    private const int leftMarginName = 810;
    private const int bottomMarginName = 750;
    public const int MaxNameWidth = 1670;
    public const int FontSizeName = 175;
    private const int leftMarginDate = leftMarginName;
    private const int bottomMarginDate = 460;
    private const int fontSizeDate = 50;
    private const int jpgQuality = 94;

    public CertificateGenerator(ITemplateBitmapRetriever templateBitmapRetriever)
    {
        _templateBitmapRetriever = templateBitmapRetriever;

        RobotoSlabTypefaceMedium = ReadFontFromEmbeddedResource("CertificateGeneratorCore.fonts.RobotoSlab-Medium.ttf");
        _robotoSlabTypefaceRegular = ReadFontFromEmbeddedResource("CertificateGeneratorCore.fonts.RobotoSlab-Regular.ttf");
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

        SKData imageDataPng = CreatePngFromBitmap(bitmap);
        SKData imageDataJpg = CreateJpgFromBitmap(bitmap);

        return new Result
        {
            FullSizePngStream = CreateMemoryStreamFromImageData(imageDataPng),
            Jpg3MbStream = CreateMemoryStreamFromImageData(imageDataJpg)
        };
    }

    private void RenderSquareMeters(SKCanvas canvas, SKBitmap bitmap, int squareMeters)
    {
        const int dropShadowDelta = 15;

        SKColor textColor = _areaColors.Last(kv => kv.Key <= squareMeters).Value;
        int fontSize = _areaFontsizes.Last(kv => kv.Key <= squareMeters).Value;

        var paint = new SKPaint
        {
            Color = textColor,
            TextSize = fontSize,
            IsAntialias = true,
            Typeface = _robotoSlabTypefaceRegular
        };
        var paintDropShadow = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = fontSize,
            IsAntialias = true,
            Typeface = _robotoSlabTypefaceRegular
        };

        string m2Text = $"m²";
        float m2TextSize = paint.MeasureText(m2Text);
        var point = new SKPoint(bitmap.Width - rightMarginSquareMeters - m2TextSize, topMarginSquareMeters + fontSize);
        var pointDropShadow = new SKPoint(bitmap.Width - rightMarginSquareMeters - m2TextSize + dropShadowDelta,
                                          topMarginSquareMeters + fontSize + dropShadowDelta);
        canvas.DrawText(m2Text, pointDropShadow, paintDropShadow);
        canvas.DrawText(m2Text, point, paint);

        paint.Typeface = RobotoSlabTypefaceMedium;
        paintDropShadow.Typeface = RobotoSlabTypefaceMedium;
        string text = $"{squareMeters}";
        float textSize = paint.MeasureText(text);

        point = new SKPoint(bitmap.Width - rightMarginSquareMeters - m2TextSize - textSize, topMarginSquareMeters + fontSize);
        pointDropShadow = new SKPoint(bitmap.Width - rightMarginSquareMeters - m2TextSize - textSize + dropShadowDelta,
                                      topMarginSquareMeters + fontSize + dropShadowDelta);
        canvas.DrawText(text, pointDropShadow, paintDropShadow);
        canvas.DrawText(text, point, paint);
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
        var point = new SKPoint(leftMarginName, bitmap.Height - bottomMarginName + firstLineYOffset);
        canvas.DrawText(wrappedLines[0], point, paint);

        if (wrappedLines.Count > 1)
        {
            point = new SKPoint(leftMarginName, bitmap.Height - bottomMarginName + FontSizeName);
            canvas.DrawText(wrappedLines[1], point, paint);
        }

        if (wrappedLines.Count > 2)
        {
            throw new NotImplementedException("No code for very long lines yet.");
        }
    }


    private void RenderDate(SKCanvas canvas, SKBitmap bitmap, DateOnly date)
    {
        var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = fontSizeDate,
            IsAntialias = true,
            Typeface = _robotoSlabTypefaceRegular
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

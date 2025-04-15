using System.Drawing;
using System.Reflection;
using SkiaSharp;

namespace CertificateGeneratorCore;

public class CertificateGenerator
{
    public class Result
    {
        public required Stream Jpg3MbStream { get; set; }
    }

    private readonly IBitmapRetriever _bitmapRetriever;
    private readonly BitmapCenterer _bitmapCenterer;
    public SKTypeface RobotoSlabTypefaceMedium { get; private set; }
    public SKTypeface RobotoSlabTypefaceRegular { get; private set; }
    public CertificateTemplateConfig Config { get; private set; }

    private const int jpgQuality = 94;

    public CertificateGenerator(IBitmapRetriever bitmapRetriever, CertificateTemplateConfig config)
    {
        _bitmapRetriever = bitmapRetriever;

        RobotoSlabTypefaceMedium = ReadFontFromEmbeddedResource("CertificateGeneratorCore.fonts.RobotoSlab-Medium.ttf");
        RobotoSlabTypefaceRegular = ReadFontFromEmbeddedResource("CertificateGeneratorCore.fonts.RobotoSlab-Regular.ttf");

        Config = config;
        _bitmapCenterer = new BitmapCenterer(Config.LogoBoundingBox);
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

    public Result GenerateJpg(AdoptionRecord adoptionRecord)
    {
        using var bitmap = GenerateBitmap(adoptionRecord);
        SKData imageDataJpg = CreateJpgFromBitmap(bitmap);

        return new Result
        {
            Jpg3MbStream = CreateMemoryStreamFromImageData(imageDataJpg)
        };
    }

    public SKBitmap GenerateBitmap(AdoptionRecord adoptionRecord)
    {
        var bitmap = _bitmapRetriever.RetrieveTemplate(adoptionRecord.SquareMeters, adoptionRecord.Language).Copy();
        using var canvas = new SKCanvas(bitmap);

        RenderSquareMeters(canvas, bitmap, adoptionRecord.SquareMeters);
        RenderName(canvas, bitmap, adoptionRecord.Name);
        RenderDate(canvas, bitmap, adoptionRecord.Date);
        RenderLogo(canvas);

        return bitmap;
    }

    private void RenderLogo(SKCanvas canvas)
    {
        SKBitmap? logoBitmap = _bitmapRetriever.RetrieveLogo();
        if (logoBitmap == null)
        {
            return;
        }

        var boundingBox = Config.LogoBoundingBox;
        if (boundingBox.Width == 0 || boundingBox.Height == 0)
        {
            return;
        }

        Rectangle logoDrawRect = _bitmapCenterer.Center(logoBitmap);
        SKRect skDrawRect = new(
            logoDrawRect.X,
            logoDrawRect.Y,
            logoDrawRect.X + logoDrawRect.Width,
            logoDrawRect.Y + logoDrawRect.Height);
        canvas.DrawBitmap(logoBitmap, skDrawRect, new SKPaint
        {
            FilterQuality = SKFilterQuality.High,
            IsAntialias = true
        });
    }

    private void RenderSquareMeters(SKCanvas canvas, SKBitmap bitmap, int squareMeters)
    {
        const int dropShadowDelta = 15;
        const int dropShadowSigma = 15;

        CertificateTemplateType templateType = CertificateTemplateTypeHelper.GetTypeFromAreaSize(squareMeters);
        SKColor textColor = ConvertToSkColor(Config[templateType].AreaColor);
        int fontSize = Config[templateType].AreaFontSize;

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
        var point = new SKPoint(bitmap.Width - Config.AreaRightMargin - m2TextSize, Config.AreaTopMargin + fontSize);
        canvas.DrawText(m2Text, point, paintForRender);

        paintForMeasure.Typeface = RobotoSlabTypefaceMedium;
        paintForRender.Typeface = RobotoSlabTypefaceMedium;
        string text = $"{squareMeters}";
        float textSize = paintForMeasure.MeasureText(text);

        point = new SKPoint(bitmap.Width - Config.AreaRightMargin - m2TextSize - textSize, Config.AreaTopMargin + fontSize);
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
            TextSize = Config.NameFontSize,
            IsAntialias = true,
            Typeface = RobotoSlabTypefaceMedium
        };

        IList<string> wrappedLines = TextWrapper.WrapText(name, paint, Config.NameMaxWidth);

        int firstLineYOffset = 0;
        if (wrappedLines.Count == 1)
        {
            firstLineYOffset = Config.NameFontSize / 2;
        }
        var point = new SKPoint(Config.NameLeftMargin, bitmap.Height - Config.NameBottomMargin + firstLineYOffset);
        canvas.DrawText(wrappedLines[0], point, paint);

        if (wrappedLines.Count > 1)
        {
            point = new SKPoint(Config.NameLeftMargin, bitmap.Height - Config.NameBottomMargin + Config.NameFontSize);
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
            TextSize = Config.DateFontSize,
            IsAntialias = true,
            Typeface = RobotoSlabTypefaceRegular
        };

        var point = new SKPoint(Config.DateLeftMargin, bitmap.Height - Config.DateBottomMargin);
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

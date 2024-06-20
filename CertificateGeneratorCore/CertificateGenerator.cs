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

    private readonly SKBitmap _certificateTemplateBitmap;
    public SKTypeface RobotoSlabTypefaceMedium { get; private set; }
    private readonly SKTypeface _robotoSlabTypefaceRegular;

    private const int expectedWidth = 3507;
    private const int expectedHeight = 2480;
    private const int rightMarginSquareMeters = 60;
    private const int topMarginSquareMeters = 0;
    private const int fontSizeSquareMeters = 330;
    private const int leftMarginName = 810;
    private const int bottomMarginName = 750;
    public const int MaxNameWidth = 1670;
    public const int FontSizeName = 175;
    private const int leftMarginDate = leftMarginName;
    private const int bottomMarginDate = 460;
    private const int fontSizeDate = 50;

    public CertificateGenerator(Stream certificateTemplateStream)
    {
        _certificateTemplateBitmap = ReadBitmapFromStream(certificateTemplateStream);
        if (_certificateTemplateBitmap.Width != expectedWidth || _certificateTemplateBitmap.Height != expectedHeight)
        {
            throw new InvalidDataException($"Template image is not {expectedWidth}x{expectedHeight}");
        }

        RobotoSlabTypefaceMedium = ReadFontFromEmbeddedResource("CertificateGeneratorCore.fonts.RobotoSlab-Medium.ttf");
        _robotoSlabTypefaceRegular = ReadFontFromEmbeddedResource("CertificateGeneratorCore.fonts.RobotoSlab-Regular.ttf");
    }

    private static SKBitmap ReadBitmapFromStream(Stream certificateTemplateStream)
    {
        using var inputStream = new SKManagedStream(certificateTemplateStream);
        return SKBitmap.Decode(inputStream);
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
        using var bitmap = _certificateTemplateBitmap.Copy();
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
        var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = fontSizeSquareMeters,
            IsAntialias = true,
            Typeface = _robotoSlabTypefaceRegular
        };

        string m2Text = $"m²";
        float m2TextSize = paint.MeasureText(m2Text);
        var point = new SKPoint(bitmap.Width - rightMarginSquareMeters - m2TextSize, topMarginSquareMeters + fontSizeSquareMeters);
        canvas.DrawText(m2Text, point, paint);

        paint.Typeface = RobotoSlabTypefaceMedium;
        string text = $"{squareMeters}";
        float textSize = paint.MeasureText(text);

        point = new SKPoint(bitmap.Width - rightMarginSquareMeters - m2TextSize - textSize, topMarginSquareMeters + fontSizeSquareMeters);
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

        IList<string> wrappedLines = WrapText(name, paint, MaxNameWidth);

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

    public static IList<string> WrapText(string text, SKPaint paint, int maxWidth)
    {
        float width = paint.MeasureText(text);
        if (width <= maxWidth)
        {
            return [text];
        }

        List<string> wrappedLines = [];
        string[] parts = text.Split(' ');
        int nrParts = parts.Length;
        int currentPart = 0;

        while (currentPart < nrParts)
        {
            string currentLine = parts[currentPart];

            if (currentPart == nrParts - 1)
            {
                wrappedLines.Add(currentLine);
                break;
            }

            string nextPart = parts[currentPart + 1];
            string textToCheck = $"{currentLine} {nextPart}";
            while (paint.MeasureText(textToCheck) <= maxWidth)
            {
                currentLine = textToCheck;
                currentPart++;

                if (currentPart == nrParts - 1)
                {
                    break;
                }

                nextPart = parts[currentPart + 1];
                textToCheck = $"{currentLine} {nextPart}";
            }

            wrappedLines.Add(currentLine);
            currentPart++;
        }

        return wrappedLines;
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
        return image.Encode(SKEncodedImageFormat.Jpeg, 97);
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

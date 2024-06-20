using SkiaSharp;

namespace CertificateGeneratorCore;


public interface ITemplateBitmapRetriever
{
    SKBitmap Retrieve(int squareMeters, Language language);
}

public class FileTemplateBitmapRetriever : ITemplateBitmapRetriever
{
    private readonly SKBitmap _certificateTemplateBitmap;

    private const int ExpectedWidth = 3507;
    private const int ExpectedHeight = 2480;

    public FileTemplateBitmapRetriever(Stream certificateTemplateStream)
    {
        _certificateTemplateBitmap = ReadBitmapFromStream(certificateTemplateStream);
        if (_certificateTemplateBitmap.Width != ExpectedWidth || _certificateTemplateBitmap.Height != ExpectedHeight)
        {
            throw new InvalidDataException($"Template image is not {ExpectedWidth}x{ExpectedHeight}");
        }
    }

    public SKBitmap Retrieve(int squareMeters, Language language)
    {
        return _certificateTemplateBitmap;
    }

    private static SKBitmap ReadBitmapFromStream(Stream certificateTemplateStream)
    {
        using var inputStream = new SKManagedStream(certificateTemplateStream);
        return SKBitmap.Decode(inputStream);
    }
}
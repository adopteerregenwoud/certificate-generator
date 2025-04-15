using SkiaSharp;

namespace CertificateGeneratorCoreTests;

public class DummyBitmapRetriever : IBitmapRetriever
{
    private readonly SKBitmap _certificateTemplateBitmap;

    public DummyBitmapRetriever()
    {
        _certificateTemplateBitmap = ImageUtils.CreateBlackBitmap();
    }

    public void Dispose()
    {
        _certificateTemplateBitmap.Dispose();
        GC.SuppressFinalize(this);
    }

    public SKBitmap RetrieveTemplate(int squareMeters, Language language)
    {
        return _certificateTemplateBitmap;
    }
}

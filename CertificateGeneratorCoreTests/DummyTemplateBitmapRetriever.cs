using SkiaSharp;

namespace CertificateGeneratorCoreTests;

public class DummyTemplateBitmapRetriever : ITemplateBitmapRetriever
{
    private readonly SKBitmap _certificateTemplateBitmap;

    public DummyTemplateBitmapRetriever()
    {
        _certificateTemplateBitmap = ImageUtils.CreateBlackBitmap();
    }

    public void Dispose()
    {
        _certificateTemplateBitmap.Dispose();
        GC.SuppressFinalize(this);
    }

    public SKBitmap Retrieve(int squareMeters, Language language)
    {
        return _certificateTemplateBitmap;
    }
}

using SkiaSharp;

namespace CertificateGeneratorCoreTests;

public class DummyTemplateBitmapReceiver : ITemplateBitmapRetriever
{
    private readonly SKBitmap _certificateTemplateBitmap;

    public DummyTemplateBitmapReceiver()
    {
        _certificateTemplateBitmap = ImageUtils.CreateBlackBitmap();
    }

    public SKBitmap Retrieve(int squareMeters, Language language)
    {
        return _certificateTemplateBitmap;
    }
}

using SkiaSharp;

namespace CertificateGeneratorCoreTests;

public class DummyBitmapRetriever : IBitmapRetriever
{
    private readonly SKBitmap _certificateTemplateBitmap;
    private readonly SKBitmap _logoBitmap;

    public DummyBitmapRetriever()
    {
        _certificateTemplateBitmap = ImageUtils.CreateBlackBitmap();
        _logoBitmap = ImageUtils.CreateBitmap(100, 100, SKColors.Red);
    }

    public void Dispose()
    {
        _certificateTemplateBitmap.Dispose();
        _logoBitmap.Dispose();
        GC.SuppressFinalize(this);
    }

    public SKBitmap? RetrieveLogo()
    {
        return _logoBitmap;
    }

    public SKBitmap RetrieveTemplate(int squareMeters, Language language)
    {
        return _certificateTemplateBitmap;
    }
}

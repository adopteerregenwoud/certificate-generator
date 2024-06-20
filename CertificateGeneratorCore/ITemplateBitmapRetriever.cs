using SkiaSharp;

namespace CertificateGeneratorCore;


public interface ITemplateBitmapRetriever
{
    SKBitmap Retrieve(int squareMeters, Language language);
}

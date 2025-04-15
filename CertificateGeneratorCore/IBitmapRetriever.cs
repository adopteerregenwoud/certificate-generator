using SkiaSharp;

namespace CertificateGeneratorCore;

/// <summary>
/// Interface to retrieve a template bitmap given the square meters that are
/// adopted and the language of the certificate.
/// </summary>
public interface IBitmapRetriever : IDisposable
{
    SKBitmap? RetrieveLogo();
    SKBitmap RetrieveTemplate(int squareMeters, Language language);
}

using SkiaSharp;

namespace CertificateGeneratorCore;

/// <summary>
/// Interface to retrieve a template bitmap given the square meters that are
/// adopted and the language of the certificate.
/// </summary>
public interface ITemplateBitmapRetriever : IDisposable
{
    SKBitmap Retrieve(int squareMeters, Language language);
}

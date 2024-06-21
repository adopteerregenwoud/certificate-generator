using SkiaSharp;

namespace CertificateGeneratorCore;

/// <summary>
/// Retrieves template bitmaps by reading png files from the file system.
/// Input is a directory path. Within this directory these files should be present:
/// 1-dutch.png
/// 1-english.png
/// 20-dutch.png
/// 20-english.png
/// TODO: add more
/// </summary>
public class FileTemplateBitmapRetriever : ITemplateBitmapRetriever
{
    private readonly SKBitmap _certificateTemplateBitmap;
    private bool _disposed = false;

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
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(FileTemplateBitmapRetriever));
        }

        return _certificateTemplateBitmap;
    }

    private static SKBitmap ReadBitmapFromStream(Stream certificateTemplateStream)
    {
        using var inputStream = new SKManagedStream(certificateTemplateStream);
        return SKBitmap.Decode(inputStream);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _certificateTemplateBitmap.Dispose();
            }

            _disposed = true;
        }
    }

    ~FileTemplateBitmapRetriever()
    {
        Dispose(false);
    }
}
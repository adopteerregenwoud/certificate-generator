using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SkiaSharp;
using Serilog;

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
    private readonly SKBitmap _fallbackCertificateTemplateBitmap;

    /// <summary>
    /// This will store the template bitmap for a specific language & area combination.
    /// </summary>
    private readonly Dictionary<Language, Dictionary<int, SKBitmap>> _certificateTemplateBitmaps = new()
    {
        [Language.Dutch] = [],
        [Language.English] = []
    };

    /// <summary>
    /// Supported languages.
    /// </summary>
    private readonly List<Language> _languages = [Language.Dutch, Language.English];

    /// <summary>
    /// Supported areas.
    /// </summary>
    private readonly List<int> _areasM2 = [1, 4, 10, 20, 50, 100];

    private bool _disposed = false;

    private const int ExpectedWidth = 3507;
    private const int ExpectedHeight = 2480;

    public FileTemplateBitmapRetriever(string templateDirectoryPath)
    {
        foreach (Language language in _languages)
        {
            foreach (int areaM2 in _areasM2)
            {
                string templatePath = Path.Combine(templateDirectoryPath, $"{areaM2}-{language.ToString().ToLower()}.png");
                Serilog.Log.Information("Reading template bitmap for {areaM2}m2 in {language} from {templatePath}...", areaM2, language, templatePath);
                using var stream = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
                SKBitmap templateBitmap = ReadBitmapFromStream(stream);
                if (templateBitmap.Width != ExpectedWidth || templateBitmap.Height != ExpectedHeight)
                {
                    Serilog.Log.Error("Template image {templatePath} is not {ExpectedWidth}x{ExpectedHeight}", templatePath, ExpectedWidth, ExpectedHeight);
                    throw new InvalidDataException($"Template image {templatePath} is not {ExpectedWidth}x{ExpectedHeight}");
                }
                _certificateTemplateBitmaps[language][areaM2] = templateBitmap;
            }
        }

        _fallbackCertificateTemplateBitmap = _certificateTemplateBitmaps[Language.Dutch][20];
    }

    public SKBitmap Retrieve(int squareMeters, Language language)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(FileTemplateBitmapRetriever));
        }

        return GetBitmap(squareMeters, language);
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
                _fallbackCertificateTemplateBitmap.Dispose();
            }

            _disposed = true;
        }
    }

    ~FileTemplateBitmapRetriever()
    {
        Dispose(false);
    }

    private SKBitmap GetBitmap(int areaM2, Language language)
    {
        if (_certificateTemplateBitmaps.ContainsKey(language))
        {
            int templateAreaM2 = _areasM2.Last(a => a <= areaM2);
            return _certificateTemplateBitmaps[language][templateAreaM2];
        }

        return _fallbackCertificateTemplateBitmap;
    }
}

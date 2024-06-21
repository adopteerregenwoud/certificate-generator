namespace CertificateGeneratorCore;

public static class CertificateUtils
{
    public static void GenerateCertificate(AdoptionRecord adoptionRecord, CertificateGenerator certificateGenerator, string outputDirectory)
    {
        CertificateGenerator.Result result = certificateGenerator.Generate(adoptionRecord);

        string fileBasename = GenerateBasename(adoptionRecord);
        string dir = Path.Combine(outputDirectory, fileBasename);
        Directory.CreateDirectory(dir);

        string outputPath = Path.Combine(dir, "certificaat.png");
        Console.WriteLine($"    Writing full-size PNG to {outputPath}...");
        using var outputStream = File.Create(outputPath);
        result.FullSizePngStream.Seek(0, SeekOrigin.Begin);
        result.FullSizePngStream.CopyTo(outputStream);

        outputPath = Path.Combine(dir, "certificaat.jpg");
        Console.WriteLine($"    Writing 3MB JPG to {outputPath}...");
        using var outputStreamJpg = File.Create(outputPath);
        result.Jpg3MbStream.Seek(0, SeekOrigin.Begin);
        result.Jpg3MbStream.CopyTo(outputStreamJpg);
    }

    private static string GenerateBasename(AdoptionRecord adoptionRecord)
    {
        return $"{adoptionRecord.Date:yyyy-MM-dd}-{adoptionRecord.Name}";
    }
}
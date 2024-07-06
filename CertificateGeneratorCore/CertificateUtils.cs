namespace CertificateGeneratorCore;

public static class CertificateUtils
{
    /// <summary>
    /// Generate a certificate on the file system.
    /// </summary>
    /// <param name="adoptionRecord"></param>
    /// <param name="certificateGenerator"></param>
    /// <param name="outputDirectory"></param>
    /// <returns>The path to the generate jpg file.</returns>
    public static string GenerateCertificate(AdoptionRecord adoptionRecord, CertificateGenerator certificateGenerator, string outputDirectory)
    {
        CertificateGenerator.Result result = certificateGenerator.Generate(adoptionRecord);

        string fileBasename = GenerateBasename(adoptionRecord);
        string outputPath = Path.Combine(outputDirectory, $"{fileBasename}.jpg");
        Console.WriteLine($"    Writing JPG to {outputPath}...");
        using var outputStreamJpg = File.Create(outputPath);
        result.Jpg3MbStream.Seek(0, SeekOrigin.Begin);
        result.Jpg3MbStream.CopyTo(outputStreamJpg);

        return outputPath;
    }

    private static string GenerateBasename(AdoptionRecord adoptionRecord)
    {
        switch (adoptionRecord.Language)
        {
            case Language.Dutch:
                return $"Certificaat Adopteer Regenwoud - {adoptionRecord.Name}";
            case Language.English:
                return $"Certificate Adopt Rainforest - {adoptionRecord.Name}";
            default:
                throw new NotImplementedException($"Unknown language {adoptionRecord.Language}");
        }
    }
}

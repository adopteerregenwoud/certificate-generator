using CertificateGeneratorCore;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: CertificateGeneratorConsole <path to template> <output directory>");
            Environment.Exit(1);
        }

        string templatePath = args[0];
        string outputDirectory = args[1];

        Stream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
        // Stream template = ImageUtils.CreateBlackTemplate();
        var certificateGenerator = new CertificateGenerator(template);
        var adoptionRecord = new AdoptionRecord("Janssen", 20, new DateOnly(2024, 6, 19), Language.Dutch);

        Stream resultStream = certificateGenerator.Generate(adoptionRecord);

        string outputPath = Path.Combine(outputDirectory, "certificate.png");
        using var outputStream = File.Create(outputPath);
        resultStream.Seek(0, SeekOrigin.Begin);
        resultStream.CopyTo(outputStream);
    }
}

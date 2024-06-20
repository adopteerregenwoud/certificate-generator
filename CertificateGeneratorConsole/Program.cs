using CertificateGeneratorCore;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: CertificateGeneratorConsole <path to excel> <path to template> <output directory>");
            Environment.Exit(1);
        }

        string excelPath = args[0];
        string templatePath = args[1];
        string outputDirectory = args[2];

        Console.WriteLine($"Reading certificate template from {templatePath}...");
        Stream template = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
        var certificateGenerator = new CertificateGenerator(template);

        Console.WriteLine($"Reading records from {excelPath}...");
        IEnumerable<AdoptionRecord> adoptionRecords = ParseExcel(excelPath);
        foreach (AdoptionRecord adoptionRecord in adoptionRecords)
        {
            Console.WriteLine($"Generating certificate for {adoptionRecord.Name} - {adoptionRecord.Date:dd-MM-yyyy} - {adoptionRecord.SquareMeters}m2 in {adoptionRecord.Language}...");
            GenerateCertificate(adoptionRecord, certificateGenerator, outputDirectory);
        }
    }

    private static void GenerateCertificate(AdoptionRecord adoptionRecord, CertificateGenerator certificateGenerator, string outputDirectory)
    {
        Stream resultStream = certificateGenerator.Generate(adoptionRecord);

        string fileBasename = GenerateFileBasename(adoptionRecord);

        string outputPath = Path.Combine(outputDirectory, $"{fileBasename}.png");
        Console.WriteLine($"    Writing full-size PNG to {outputPath}...");
        using var outputStream = File.Create(outputPath);
        resultStream.Seek(0, SeekOrigin.Begin);
        resultStream.CopyTo(outputStream);
    }

    private static string GenerateFileBasename(AdoptionRecord adoptionRecord)
    {
        return $"certificate-{adoptionRecord.Name}-{adoptionRecord.Date:yyyy-MM-dd}";
    }

    private static IEnumerable<AdoptionRecord> ParseExcel(string excelPath)
    {
        FileStream stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read);
        var parser = new ExcelAdoptionRecordsParser(stream);
        return parser.Parse();
    }
}

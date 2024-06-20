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
        using var templateBitmapRetriever = new FileTemplateBitmapRetriever(template);
        var certificateGenerator = new CertificateGenerator(templateBitmapRetriever);

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

    private static IEnumerable<AdoptionRecord> ParseExcel(string excelPath)
    {
        FileStream stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read);
        var parser = new ExcelAdoptionRecordsParser(stream);
        return parser.Parse();
    }
}

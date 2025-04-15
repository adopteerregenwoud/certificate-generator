using CertificateGeneratorCore;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: BulkTool <path to excel> <template directory> <logo path> <output directory>");
            Environment.Exit(1);
        }

        string excelPath = args[0];
        string templateDirectory = args[1];
        string logoPath = args[2];
        string outputDirectory = args[3];

        Console.WriteLine($"Reading certificate templates from {templateDirectory}...");
        using var bitmapRetriever = new FileBitmapRetriever(templateDirectory, logoPath);
        CertificateTemplateConfig config = GetOrCreateConfigFromTemplateDirectory(templateDirectory);
        var certificateGenerator = new CertificateGenerator(bitmapRetriever, config);

        Console.WriteLine($"Reading records from {excelPath}...");
        IEnumerable<AdoptionRecord> adoptionRecords = CertificateUtils.ParseExcelWidthAdoptionRecords(excelPath);
        foreach (AdoptionRecord adoptionRecord in adoptionRecords)
        {
            Console.WriteLine($"Generating certificate for {adoptionRecord.Name} - {adoptionRecord.Date:dd-MM-yyyy} - {adoptionRecord.SquareMeters}m2 in {adoptionRecord.Language}...");
            CertificateUtils.GenerateCertificate(adoptionRecord, certificateGenerator, outputDirectory);
        }
    }

    public static CertificateTemplateConfig GetOrCreateConfigFromTemplateDirectory(string templateDirectory)
    {
        string configPath = Path.Join(templateDirectory, "config.yml");
        if (!File.Exists(configPath))
        {
            CertificateTemplateConfig config = CertificateTemplateConfig.Default;
            File.WriteAllText(configPath, config.ToString());
            return config;
        }

        string yaml = File.ReadAllText(configPath);
        return CertificateTemplateConfig.FromYaml(yaml);
    }
}

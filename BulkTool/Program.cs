using CertificateGeneratorCore;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: BulkTool <path to excel> <template directory> <output directory>");
            Environment.Exit(1);
        }

        string excelPath = args[0];
        string templateDirectory = args[1];
        string outputDirectory = args[2];

        Console.WriteLine($"Reading certificate templates from {templateDirectory}...");
        using var templateBitmapRetriever = new FileTemplateBitmapRetriever(templateDirectory);
        CertificateTemplateConfig config = GetOrCreateConfigFromTemplateDirectory(templateDirectory);
        var certificateGenerator = new CertificateGenerator(templateBitmapRetriever, config);

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

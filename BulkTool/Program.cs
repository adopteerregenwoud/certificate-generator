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
        var certificateGenerator = new CertificateGenerator(templateBitmapRetriever, CertificateTemplateConfig.Default);

        Console.WriteLine($"Reading records from {excelPath}...");
        IEnumerable<AdoptionRecord> adoptionRecords = CertificateUtils.ParseExcelWidthAdoptionRecords(excelPath);
        foreach (AdoptionRecord adoptionRecord in adoptionRecords)
        {
            Console.WriteLine($"Generating certificate for {adoptionRecord.Name} - {adoptionRecord.Date:dd-MM-yyyy} - {adoptionRecord.SquareMeters}m2 in {adoptionRecord.Language}...");
            CertificateUtils.GenerateCertificate(adoptionRecord, certificateGenerator, outputDirectory);
        }
    }
}

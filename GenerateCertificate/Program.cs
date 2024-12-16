using CertificateGeneratorCore;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 6)
        {
            Console.WriteLine("Usage: GenerateCertificate <template directory> <output directory> <name> <square meters> <date> <language>");
            Environment.Exit(1);
        }

        string templateDirectory = args[0];
        string outputDirectory = args[1];
        string name = args[2];
        int squareMeters = int.Parse(args[3]);
        string date = args[4];
        Language language = (Language)Enum.Parse(typeof(Language), args[5]);

        Console.WriteLine($"Reading certificate templates from {templateDirectory}...");
        using var templateBitmapRetriever = new FileTemplateBitmapRetriever(templateDirectory);
        var certificateGenerator = new CertificateGenerator(templateBitmapRetriever, CertificateTemplateConfig.Default);

        AdoptionRecord adoptionRecord = new(name, squareMeters, date, language);
        CertificateUtils.GenerateCertificate(adoptionRecord, certificateGenerator, outputDirectory);
    }
}
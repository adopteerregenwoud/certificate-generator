﻿using CertificateGeneratorCore;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: CertificateGeneratorConsole <path to excel> <template directory> <output directory>");
            Environment.Exit(1);
        }

        string excelPath = args[0];
        string templateDirectory = args[1];
        string outputDirectory = args[2];

        Console.WriteLine($"Reading certificate templates from {templateDirectory}...");
        using var templateBitmapRetriever = new FileTemplateBitmapRetriever(templateDirectory);
        var certificateGenerator = new CertificateGenerator(templateBitmapRetriever);

        Console.WriteLine($"Reading records from {excelPath}...");
        IEnumerable<AdoptionRecord> adoptionRecords = ParseExcel(excelPath);
        foreach (AdoptionRecord adoptionRecord in adoptionRecords)
        {
            Console.WriteLine($"Generating certificate for {adoptionRecord.Name} - {adoptionRecord.Date:dd-MM-yyyy} - {adoptionRecord.SquareMeters}m2 in {adoptionRecord.Language}...");
            CertificateUtils.GenerateCertificate(adoptionRecord, certificateGenerator, outputDirectory);
        }
    }

    private static IEnumerable<AdoptionRecord> ParseExcel(string excelPath)
    {
        FileStream stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read);
        var parser = new ExcelAdoptionRecordsParser(stream);
        return parser.Parse();
    }
}

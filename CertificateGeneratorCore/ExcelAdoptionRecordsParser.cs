using OfficeOpenXml;
using EPPlus.DataExtractor;

namespace CertificateGeneratorCore;

public class ExcelAdoptionRecordsParser : IAdoptionRecordsParser
{
    private readonly Stream _stream;

    public ExcelAdoptionRecordsParser(Stream stream)
    {
        this._stream = stream;
    }

    public IEnumerable<AdoptionRecord> Parse()
    {
        using (var ep = new ExcelPackage(_stream))
        {
            ExcelWorksheet ws = ep.Workbook.Worksheets.First();
            int headerRow = 1;
            int lastRow = GetLastRowWithData(ws);

            string columnName = ws.FindColumnName(headerRow, ["Naam", "Name"]);
            string columnSquareMeters = ws.FindColumnName(headerRow,
                ["Aantal m2", "m2", "vierkante meter", "vierkante meters", "square meters"]);
            string columnDate = ws.FindColumnName(headerRow, ["Datum", "date"]);
            string columnLanguage = ws.FindColumnName(headerRow, ["Taal", "language"]);

            return new List<AdoptionRecord>(ws.Extract<AdoptionRecord>()
                .WithProperty(p => p.Name, columnName)
                .WithProperty(p => p.SquareMeters, columnSquareMeters)
                .WithProperty(p => p.Date, columnDate)
                .WithProperty(p => p.Language, columnLanguage, cell => Enum.Parse<Language>((string)cell))
                .GetData(headerRow + 1, lastRow));
        }
    }

    private static int GetLastRowWithData(ExcelWorksheet ws)
    {
        int row = 1;
        while (row <= ws.Dimension.End.Row)
        {
            string? value = ws.GetValue<string?>(row, 1);
            if (value == null || string.IsNullOrEmpty(value))
            {
                return row - 1;
            }

            row++;
        }

        return ws.Dimension.End.Row;
    }
}

using OfficeOpenXml;

namespace CertificateGeneratorCore;

public static class EPPlusExtensions
{
    /// <summary>
    /// Find a column name given one or more possible column headers.
    /// </summary>
    /// <param name="ws">Worksheet to read the information from.</param>
    /// <param name="headerRow">Index (1-based) of the row that contains the header.</param>
    /// <param name="headerAlternatives">Name of column header with alternatives.</param>
    /// <returns>Name of the column, for example "A" or "H".</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static string FindColumnName(this ExcelWorksheet ws, int headerRow, IEnumerable<string> headerAlternatives)
    {
        int lastColumn = ws.Dimension.End.Column;
        const int nrLettersInAlphabet = 26;
        if (lastColumn > nrLettersInAlphabet)
        {
            throw new NotImplementedException("FindColumnName does not support more than 26 columns");
        }

        for (int columnIndex = 1; columnIndex <= lastColumn; columnIndex++)
        {
            foreach (var headerAlternative in headerAlternatives)
            {
                string? cellText = ws.Cells[headerRow, columnIndex].Value as string;
                if (string.Equals(cellText, headerAlternative, StringComparison.InvariantCultureIgnoreCase))
                {
                    return ((char)('A' + columnIndex - 1)).ToString();
                }
            }
        }
        string columnHeadersText = string.Join(", ", headerAlternatives);
        throw new KeyNotFoundException($"Unknown column {columnHeadersText}");
    }

    public static int FindColumnIndex(this ExcelWorksheet ws, int columnRow, IEnumerable<string> columnHeaders)
    {
        int lastColumn = ws.Dimension.End.Column;
        for (int i = 1; i <= lastColumn; i++)
        {
            foreach (var columnHeader in columnHeaders)
            {
                if (ws.Cells[columnRow, i].Value as string == columnHeader)
                {
                    return i;
                }
            }
        }
        string columnHeadersText = string.Join(", ", columnHeaders);
        throw new KeyNotFoundException($"Unknown column {columnHeadersText}");
    }
}

using OfficeOpenXml;

static class EPPlusExtensions
{
    public static string FindColumnName(this ExcelWorksheet ws, int columnRow, string columnHeader)
    {
        int lastColumn = ws.Dimension.End.Column;
        for (int i = 1; i <= lastColumn; i++)
        {
            if (ws.Cells[columnRow, i].Value as string == columnHeader)
            {
                return ((char)('A' - 1 + i)).ToString();
            }
        }
        string columnHeadersText = string.Join(", ", columnHeader);
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

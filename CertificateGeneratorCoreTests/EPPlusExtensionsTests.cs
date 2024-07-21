using OfficeOpenXml;

namespace CertificateGeneratorCoreTests;

public class EPPlusExtensionsTests
{
    [TestCase("Aantal m2", true)]
    [TestCase("aantal m2", true)]
    [TestCase("m2", true)]
    [TestCase("M2", true)]
    [TestCase("Aantal", false)]
    [TestCase("Header 1", false)]
    public void TestFindColumnName(string headerText, bool shouldMatch)
    {
        // Arrange
        using var package = new ExcelPackage();
        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
        worksheet.Cells[1, 1].Value = "Header 1";
        worksheet.Cells[1, 2].Value = "Header 2";
        worksheet.Cells[1, 3].Value = headerText;
        const int headerRow = 1;

        // Act
        if (shouldMatch)
        {
            string columnName = worksheet.FindColumnName(headerRow, ["Aantal m2", "m2", "vierkante meter", "vierkante meters"]);
            Assert.That(columnName, Is.EqualTo("C"));
        }
        else
        {
            try
            {
                worksheet.FindColumnName(headerRow, ["Aantal m2", "m2", "vierkante meter", "vierkante meters"]);
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
                Assert.Pass();
            }
        }
    }
}

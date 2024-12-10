namespace CertificateGeneratorCoreTests;

public class ExcelAdoptionRecordsParserTests
{
    [Test]
    public void TestParse()
    {
        // Arrange
        var path = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "example.xlsx");
        var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var sut = new ExcelAdoptionRecordsParser(stream);

        // Act
        List<AdoptionRecord> actualRecords = sut.Parse().ToList();

        // Assert
        var expectedRecords = new List<AdoptionRecord>
        {
            new ("John Doe", 3, "2023-06-01", Language.English),
            new ("Jane Smith en ook nog vele anderen", 200, "2023-06-02", Language.Dutch),
            new ("Max Mustermann", 180, "2023-06-03", Language.English),
            new ("María García", 5, "2023-06-04", Language.Dutch),
            new ("François Dupont", 20, "December 2024", Language.Dutch)
        };

        Assert.That(actualRecords, Is.EqualTo(expectedRecords));
    }
}
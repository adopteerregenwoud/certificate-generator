namespace AdopteerRegenwoudCertificateGeneratorCoreTests;

public class ExcelAdoptionRecordGeneratorTests
{
    [Test]
    public void TestGenerate()
    {
        // Arrange
        var path = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "example.xlsx");
        var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var sut = new ExcelAdoptionRecordGenerator(stream);

        // Act
        List<AdoptionRecord> actualRecords = sut.Generate().ToList();

        // Assert
        var expectedRecords = new List<AdoptionRecord>
        {
            new ("John Doe", 150,   new DateOnly(2023, 06, 01), Language.English),
            new ("Jane Smith", 200, new DateOnly(2023, 06, 02), Language.Dutch),
            new ("Max Mustermann", 180, new DateOnly(2023, 06, 03), Language.English),
            new ("María García", 220, new DateOnly(2023, 06, 04), Language.Dutch),
            new ("François Dupont", 170, new DateOnly(2023, 06, 05), Language.Dutch)
        };

        Assert.That(actualRecords, Is.EqualTo(expectedRecords));
    }
}
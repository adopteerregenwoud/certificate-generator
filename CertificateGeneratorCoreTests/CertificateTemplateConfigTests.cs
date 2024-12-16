namespace CertificateGeneratorCoreTests;

public class CertificateTemplateConfigTests
{
    private class AreaYamlItem
    {
        public int Area { get; set; }
        public int AreaFontSize { get; set; }
    }

    [Test]
    public void TestFromYaml()
    {
        // Arrange
        string yaml = """
        - area: 1
          area_font_size: 390
        - area: 4
          area_font_size: 430
        - area: 10
          area_font_size: 430
        - area: 20
          area_font_size: 360
        - area: 50
          area_font_size: 430
        - area: 100
          area_font_size: 430
        """;
        CertificateTemplateConfig expected = new()
        {
            FontSizePerType = new Dictionary<CertificateTemplateType, CertificateTemplateAreaConfig>()
            {
                [CertificateTemplateType.OneM2] = new() { AreaFontSize = 390 },
                [CertificateTemplateType.FourM2] = new() { AreaFontSize = 430 },
                [CertificateTemplateType.TenM2] = new() { AreaFontSize = 430 },
                [CertificateTemplateType.TwentyM2] = new() { AreaFontSize = 360 },
                [CertificateTemplateType.FiftyM2] = new() { AreaFontSize = 430 },
                [CertificateTemplateType.HundredM2] = new() { AreaFontSize = 430 },
            }
        };

        // Act
        CertificateTemplateConfig actual = CertificateTemplateConfig.FromYaml(yaml);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
}
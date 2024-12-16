namespace CertificateGeneratorCore;

public enum CertificateTemplateType
{
    OneM2 = 1,
    FourM2 = 4,
    TenM2 = 10,
    TwentyM2 = 20,
    FiftyM2 = 50,
    HundredM2 = 100
}

public static class CertificateTemplateTypeHelper
{
    public static CertificateTemplateType GetTypeFromAreaSize(int areaSize)
    {
        var types = Enum.GetValues(typeof(CertificateTemplateType))
                        .Cast<CertificateTemplateType>()
                        .OrderByDescending(t => (int)t);

        foreach (var type in types)
        {
            if ((int)type <= areaSize)
            {
                return type;
            }
        }

        throw new ArgumentException($"No lower or equal CertificateTemplateType found for area {areaSize}");
    }
}

using System.Text;
using YamlDotNet.Serialization;

namespace CertificateGeneratorCore;

/// <summary>
/// Configuration item for a specific area size.
/// </summary>
public class CertificateTemplateAreaConfig
{
    /// <summary>
    /// The size of the font to use to render the size of the area.
    /// </summary>
    public int AreaFontSize { get; set; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj is not CertificateTemplateAreaConfig other)
        {
            return false;
        }

        return AreaFontSize == other.AreaFontSize;
    }

    public override int GetHashCode()
    {
        return AreaFontSize.GetHashCode();
    }

    public override string ToString()
    {
        return $"{{ AreaFontSize = {AreaFontSize} }}";
    }
}

public class CertificateTemplateConfig
{
    /// <summary>
    /// Helper class to read data from yaml.
    /// </summary>
    private class YamlAreaItem
    {
        [YamlMember(Alias = "area")]
        public int Area { get; set; }

        [YamlMember(Alias = "area_font_size")]
        public int AreaFontSize { get; set; }
    }

    public Dictionary<CertificateTemplateType, CertificateTemplateAreaConfig> ConfigPerAreaType { get; set; } = [];

    public static CertificateTemplateConfig Default => new()
    {
        ConfigPerAreaType = new Dictionary<CertificateTemplateType, CertificateTemplateAreaConfig>()
        {
            [CertificateTemplateType.OneM2] = new() { AreaFontSize = 390 },
            [CertificateTemplateType.FourM2] = new() { AreaFontSize = 430 },
            [CertificateTemplateType.TenM2] = new() { AreaFontSize = 430 },
            [CertificateTemplateType.TwentyM2] = new() { AreaFontSize = 360 },
            [CertificateTemplateType.FiftyM2] = new() { AreaFontSize = 430 },
            [CertificateTemplateType.HundredM2] = new() { AreaFontSize = 430 },
        }
    };

    public static CertificateTemplateConfig FromYaml(string yaml)
    {
        var deserializer = new DeserializerBuilder().Build();
        var areaItems = deserializer.Deserialize<List<YamlAreaItem>>(yaml);
        var config = new CertificateTemplateConfig();
        foreach (var item in areaItems)
        {
            if (Enum.IsDefined(typeof(CertificateTemplateType), item.Area))
            {
                var type = (CertificateTemplateType)item.Area;
                config.ConfigPerAreaType[type] = new CertificateTemplateAreaConfig
                {
                    AreaFontSize = item.AreaFontSize
                };
            }
            else
            {
                throw new Exception($"Unknown area type: {item.Area}");
            }
        }
        return config;
    }

    public CertificateTemplateAreaConfig this[CertificateTemplateType templateType]
    {
        get
        {
            if (ConfigPerAreaType.TryGetValue(templateType, out CertificateTemplateAreaConfig? config))
            {
                return config;
            }

            throw new KeyNotFoundException($"Template type '{templateType}' is not configured.");
        }
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj is not CertificateTemplateConfig other)
        {
            return false;
        }

        return ConfigPerAreaType.Count == other.ConfigPerAreaType.Count &&
               ConfigPerAreaType.All(kvp =>
                   other.ConfigPerAreaType.TryGetValue(kvp.Key, out var otherValue) &&
                   kvp.Value.Equals(otherValue));
    }

    public override int GetHashCode()
    {
        int hash = 17;

        foreach (var kvp in ConfigPerAreaType.OrderBy(kvp => kvp.Key))
        {
            hash = hash * 31 + kvp.Key.GetHashCode();
            hash = hash * 31 + kvp.Value.GetHashCode();
        }

        return hash;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (CertificateTemplateType type in ConfigPerAreaType.Keys.Order())
        {
            sb.AppendLine($"- area: {(int)type}");
            sb.AppendLine($"  area_font_size: {ConfigPerAreaType[type]}");
        }

        return sb.ToString();
    }
}

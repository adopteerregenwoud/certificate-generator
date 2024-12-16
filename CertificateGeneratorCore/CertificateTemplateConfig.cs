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

    public Dictionary<CertificateTemplateType, CertificateTemplateAreaConfig> FontSizePerType { get; set; } = [];

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
                config.FontSizePerType[type] = new CertificateTemplateAreaConfig
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

        return FontSizePerType.Count == other.FontSizePerType.Count &&
               FontSizePerType.All(kvp =>
                   other.FontSizePerType.TryGetValue(kvp.Key, out var otherValue) &&
                   kvp.Value.Equals(otherValue));
    }

    public override int GetHashCode()
    {
        int hash = 17;

        foreach (var kvp in FontSizePerType.OrderBy(kvp => kvp.Key))
        {
            hash = hash * 31 + kvp.Key.GetHashCode();
            hash = hash * 31 + kvp.Value.GetHashCode();
        }

        return hash;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (CertificateTemplateType type in FontSizePerType.Keys.Order())
        {
            sb.AppendLine($"- area: {(int)type}");
            sb.AppendLine($"  area_font_size: {FontSizePerType[type]}");
        }

        return sb.ToString();
    }
}

using System.Text;
using YamlDotNet.Serialization;

namespace CertificateGeneratorCore;

public record class RgbColor(int R, int G, int B)
{
    public RgbColor() : this(0, 0, 0) { }

    [YamlMember(Alias = "r")]
    public int R { get; init; } = R;

    [YamlMember(Alias = "g")]
    public int G { get; init; } = G;

    [YamlMember(Alias = "b")]
    public int B { get; init; } = B;
}

/// <summary>
/// Configuration item for a specific area size.
/// </summary>
public class CertificateTemplateAreaConfig
{
    /// <summary>
    /// The size of the font to use to render the size of the area.
    /// </summary>
    public int AreaFontSize { get; set; }
    public RgbColor AreaColor { get; set; } = new(0, 0, 0);

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

        return AreaFontSize == other.AreaFontSize &&
               AreaColor == other.AreaColor;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(AreaFontSize, AreaColor);
    }

    public override string ToString()
    {
        return $"{{ AreaFontSize = {AreaFontSize}, Color = {AreaColor} }}";
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

        [YamlMember(Alias = "area_color")]
        public RgbColor AreaColor { get; set; } = new(0, 0, 0);
    }

    private class YamlConfig
    {
        [YamlMember(Alias = "areas")]
        public List<YamlAreaItem> Areas { get; set; } = new();

        [YamlMember(Alias = "area_right_margin")]
        public int AreaRightMargin { get; set; }

        [YamlMember(Alias = "area_top_margin")]
        public int AreaTopMargin { get; set; }

        [YamlMember(Alias = "name_left_margin")]
        public int NameLeftMargin { get; set; }

        [YamlMember(Alias = "name_bottom_margin")]
        public int NameBottomMargin { get; set; }

        [YamlMember(Alias = "name_font_size")]
        public int NameFontSize { get; set; }

        [YamlMember(Alias = "name_max_width")]
        public int NameMaxWidth { get; set; }

        [YamlMember(Alias = "date_left_margin")]
        public int DateLeftMargin { get; set; }

        [YamlMember(Alias = "date_bottom_margin")]
        public int DateBottomMargin { get; set; }

        [YamlMember(Alias = "date_font_size")]
        public int DateFontSize { get; set; }
    }

    // Actual configuration:
    public Dictionary<CertificateTemplateType, CertificateTemplateAreaConfig> ConfigPerAreaType { get; set; } = [];
    public int AreaRightMargin { get; set; }
    public int AreaTopMargin { get; set; }
    public int NameLeftMargin { get; set; }
    public int NameBottomMargin { get; set; }
    public int NameFontSize { get; set; }
    public int NameMaxWidth { get; set; }
    public int DateLeftMargin { get; set; }
    public int DateBottomMargin { get; set; }
    public int DateFontSize { get; set; }

    public static CertificateTemplateConfig Default => new()
    {
        ConfigPerAreaType = new Dictionary<CertificateTemplateType, CertificateTemplateAreaConfig>()
        {
            [CertificateTemplateType.OneM2] = new() { AreaFontSize = 390, AreaColor = new(196, 217, 117) },
            [CertificateTemplateType.FourM2] = new() { AreaFontSize = 430, AreaColor = new(255, 255, 255) },
            [CertificateTemplateType.TenM2] = new() { AreaFontSize = 430, AreaColor = new(196, 217, 117) },
            [CertificateTemplateType.TwentyM2] = new() { AreaFontSize = 360, AreaColor = new(255, 255, 255) },
            [CertificateTemplateType.FiftyM2] = new() { AreaFontSize = 430, AreaColor = new(196, 217, 117) },
            [CertificateTemplateType.HundredM2] = new() { AreaFontSize = 430, AreaColor = new(123, 103, 91) },
        },
        AreaRightMargin = 60,
        AreaTopMargin = 0,
        NameLeftMargin = 810,
        NameBottomMargin = 750,
        NameFontSize = 175,
        NameMaxWidth = 1670,
        DateLeftMargin = 810,
        DateBottomMargin = 460,
        DateFontSize = 50
    };

    public static CertificateTemplateConfig FromYaml(string yaml)
    {
        var deserializer = new DeserializerBuilder().Build();
        var yamlConfig = deserializer.Deserialize<YamlConfig>(yaml);
        var config = new CertificateTemplateConfig()
        {
            AreaRightMargin = yamlConfig.AreaRightMargin,
            AreaTopMargin = yamlConfig.AreaTopMargin,
            NameLeftMargin = yamlConfig.NameLeftMargin,
            NameBottomMargin = yamlConfig.NameBottomMargin,
            NameFontSize = yamlConfig.NameFontSize,
            NameMaxWidth = yamlConfig.NameMaxWidth,
            DateLeftMargin = yamlConfig.DateLeftMargin,
            DateBottomMargin = yamlConfig.DateBottomMargin,
            DateFontSize = yamlConfig.DateFontSize
        };
        foreach (var item in yamlConfig.Areas)
        {
            if (Enum.IsDefined(typeof(CertificateTemplateType), item.Area))
            {
                var type = (CertificateTemplateType)item.Area;
                config.ConfigPerAreaType[type] = new CertificateTemplateAreaConfig
                {
                    AreaFontSize = item.AreaFontSize,
                    AreaColor = item.AreaColor
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

        return AreaRightMargin == other.AreaRightMargin &&
               AreaTopMargin == other.AreaTopMargin &&
               NameLeftMargin == other.NameLeftMargin &&
               NameBottomMargin == other.NameBottomMargin &&
               NameFontSize == other.NameFontSize &&
               NameMaxWidth == other.NameMaxWidth &&
               DateLeftMargin == other.DateLeftMargin &&
               DateBottomMargin == other.DateBottomMargin &&
               DateFontSize == other.DateFontSize &&
               ConfigPerAreaType.Count == other.ConfigPerAreaType.Count &&
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

        return HashCode.Combine(hash, AreaRightMargin, AreaTopMargin,
            NameLeftMargin, NameBottomMargin, NameFontSize, NameMaxWidth,
            HashCode.Combine(DateLeftMargin, DateBottomMargin, DateFontSize));
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (CertificateTemplateType type in ConfigPerAreaType.Keys.Order())
        {
            var areaConfig = ConfigPerAreaType[type];
            sb.AppendLine("areas:");
            sb.AppendLine($"- area: {(int)type}");
            sb.AppendLine($"  area_font_size: {areaConfig.AreaFontSize}");
            sb.AppendLine($"  area_color:");
            sb.AppendLine($"    r: {areaConfig.AreaColor.R}");
            sb.AppendLine($"    g: {areaConfig.AreaColor.G}");
            sb.AppendLine($"    b: {areaConfig.AreaColor.B}");
            sb.AppendLine($"area_right_margin: {AreaRightMargin}");
            sb.AppendLine($"area_top_margin: {AreaTopMargin}");
            sb.AppendLine($"name_left_margin: {NameLeftMargin}");
            sb.AppendLine($"name_bottom_margin: {NameBottomMargin}");
            sb.AppendLine($"name_font_size: {NameFontSize}");
            sb.AppendLine($"name_max_width: {NameMaxWidth}");
            sb.AppendLine($"date_left_margin: {DateLeftMargin}");
            sb.AppendLine($"date_bottom_margin: {DateBottomMargin}");
            sb.AppendLine($"date_font_size: {DateFontSize}");
        }

        return sb.ToString();
    }
}

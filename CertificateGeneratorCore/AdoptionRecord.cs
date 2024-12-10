namespace CertificateGeneratorCore;

public enum Language
{
    Dutch,
    English
}

public record class AdoptionRecord(string Name, int SquareMeters, string Date, Language Language)
{
    public AdoptionRecord() : this(string.Empty, 0, string.Empty, Language.Dutch) { }
}

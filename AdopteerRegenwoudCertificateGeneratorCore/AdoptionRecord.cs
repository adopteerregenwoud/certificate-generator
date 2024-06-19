namespace AdopteerRegenwoud.CertificateGeneratorCore;

public enum Language
{
    Dutch,
    English
}

public class AdoptionRecord(string name, int squareMeters, DateOnly date, Language language) : IEquatable<AdoptionRecord>
{
    public string Name { get; set; } = name;
    public int SquareMeters { get; set; } = squareMeters;
    public DateOnly Date { get; set; } = date;
    public Language Language { get; set; } = language;

    public AdoptionRecord() : this(string.Empty, 0, new DateOnly(), Language.Dutch) { }

    public bool Equals(AdoptionRecord? other)
    {
        if (other == null)
        {
            return false;
        }

        return Name == other.Name &&
            SquareMeters == other.SquareMeters &&
            Date == other.Date &&
            Language == other.Language;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as AdoptionRecord);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, SquareMeters, Date, Language);
    }
}

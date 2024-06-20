namespace CertificateGeneratorCore;

interface IAdoptionRecordsParser
{
    IEnumerable<AdoptionRecord> Parse();
}

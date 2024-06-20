namespace AdopteerRegenwoud.CertificateGeneratorCore;

interface IAdoptionRecordsParser
{
    IEnumerable<AdoptionRecord> Parse();
}

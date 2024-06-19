namespace AdopteerRegenwoud.CertificateGeneratorCore;

interface IAdoptionRecordGenerator
{
    IEnumerable<AdoptionRecord> Generate();
}

using System;

namespace BulkToolUI
{
    public class CertificateModel
    {
        public string? Name { get; set; }
        public int SquareMeters { get; set; }
        public DateTime? Date { get; set; }
        public CertificateGeneratorCore.Language Language { get; set; }
    }
}

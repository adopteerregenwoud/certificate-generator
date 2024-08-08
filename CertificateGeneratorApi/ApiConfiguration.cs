namespace CertificateGeneratorApi;

public class ApiConfiguration
{
    /// <summary>
    /// Directory that contains the template .png files.
    /// </summary>
    public required string TemplateDirectory { get; set; }

    /// <summary>
    /// API token required to be sent with every request in the X-API-TOKEN request header.
    /// </summary>
    public required string ApiToken { get; set; }
}

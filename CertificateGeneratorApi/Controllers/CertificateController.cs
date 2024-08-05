using CertificateGeneratorCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CertificateGeneratorApi.Controllers;

[ApiController]
[Route("certificate")]
public class CertificateController : ControllerBase
{
    private readonly ILogger<CertificateController> _logger;
    private readonly ApiConfiguration _config;
    private readonly CertificateGenerator _certificateGenerator;

    public CertificateController(ILogger<CertificateController> logger, IOptions<ApiConfiguration> apiConfiguration)
    {
        _logger = logger;
        _config = apiConfiguration.Value;

        _logger.LogInformation("Reading certificate templates from {TemplateDirectory}", _config.TemplateDirectory);
        var templateBitmapRetriever = new FileTemplateBitmapRetriever(_config.TemplateDirectory);
        _certificateGenerator = new CertificateGenerator(templateBitmapRetriever);
    }

    [HttpGet(Name = "generate")]
    public IActionResult Get(string name, int squareMeters, int year, int month, int day, Language language)
    {
        DateOnly date = new(year, month, day);
        AdoptionRecord adoptionRecord = new(name, squareMeters, date, language);
        CertificateGenerator.Result result = _certificateGenerator.Generate(adoptionRecord);
        return new FileStreamResult(result.Jpg3MbStream, "image/jpeg");
    }
}

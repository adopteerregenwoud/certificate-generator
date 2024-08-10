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
        string obfuscatedName = $"{name.First()}{name.Length - 2}{name.Last()}";
        DateOnly date = new(year, month, day);
        _logger.LogInformation("Generating certificate for {name}, {area} m2, {date}, {language}", obfuscatedName, squareMeters, date, language);
        AdoptionRecord adoptionRecord = new(name, squareMeters, date, language);
        CertificateGenerator.Result result = _certificateGenerator.Generate(adoptionRecord);
        _logger.LogInformation("{nrBytes} bytes generated", result.Jpg3MbStream.Length);
        return new FileStreamResult(result.Jpg3MbStream, "image/jpeg");
    }
}

using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace CertificateGeneratorApi;

/// <summary>
/// Class that implements a health check that checks if the application is ready to
/// process requests by looking at:
/// - is TemplateDirectory defined?
/// - does template directory exist?
/// </summary>
public class ReadyHealthCheck : IHealthCheck
{
    private readonly ApiConfiguration _apiConfiguration;

    public ReadyHealthCheck(IOptions<ApiConfiguration> apiConfiguration)
    {
        _apiConfiguration = apiConfiguration.Value;    
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        StringBuilder errorsb = new();

        if (string.IsNullOrEmpty(_apiConfiguration.TemplateDirectory))
        {
            errorsb.AppendLine("TemplateDirectory not defined");
        }
        else
        {
            if (!Directory.Exists(_apiConfiguration.TemplateDirectory))
            {
                errorsb.AppendLine($"Template directory {_apiConfiguration.TemplateDirectory} does not exist");
            }
        }

        if (errorsb.Length > 0)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(errorsb.ToString()));
        }

        return Task.FromResult(HealthCheckResult.Healthy());
    }
}
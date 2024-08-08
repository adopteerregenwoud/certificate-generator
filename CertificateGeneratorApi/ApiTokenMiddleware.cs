using Microsoft.Extensions.Options;

namespace CertificateGeneratorApi;

/// <summary>
/// Class that verifies that a request contains an X-API-TOKEN header that
/// matches the configured API token.
/// </summary>
public class ApiTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _apiToken;

    public ApiTokenMiddleware(RequestDelegate next, IOptions<ApiConfiguration> apiConfiguration)
    {
        _next = next;
        _apiToken = apiConfiguration.Value.ApiToken;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // We don't want to complicate health checks by requiring an API token,
        // so don't check those.
        if (context.Request.Path.Value!.StartsWith("/health", StringComparison.InvariantCultureIgnoreCase))
        {
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-API-TOKEN", out var extractedToken))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Token is missing.");
            return;
        }

        if (!extractedToken.Equals(_apiToken))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        await _next(context);
    }
}

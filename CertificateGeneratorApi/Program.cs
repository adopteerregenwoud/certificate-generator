using System.Text.Json.Serialization;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog.Formatting.Json;

namespace CertificateGeneratorApi;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureLogging(builder);

        builder.Services.AddHealthChecks()
            .AddCheck<ReadyHealthCheck>("Ready", tags: ["ready"]);

        builder.Services.Configure<ApiConfiguration>(builder.Configuration.GetSection("ApiConfiguration"));

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        ;

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.UseInlineDefinitionsForEnums();
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ApiTokenMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        MapHealthChecks(app);

        app.Run();

        Log.CloseAndFlush();
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        const string logFormat = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}";
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(new JsonFormatter()/*, outputTemplate: logFormat*/)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    private static void MapHealthChecks(WebApplication app)
    {
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("ready")
        });
 
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });
    }
}
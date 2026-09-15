using System.Reflection;
using Serilog;
using Serilog.Formatting.Json;

namespace OficinaMecanica.API.Extensions.Configuration;

public static class ApiObservabilityExtensions
{
    private const string DefaultServiceName = "oficina-mecanica-api";

    public static WebApplicationBuilder AddApiObservability(this WebApplicationBuilder builder)
    {
        var environment = GetEnvironmentVariableOrDefault("DD_ENV", builder.Environment.EnvironmentName.ToLowerInvariant());
        var service = GetEnvironmentVariableOrDefault("DD_SERVICE", DefaultServiceName);
        var version = GetEnvironmentVariableOrDefault("DD_VERSION", Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown");

        builder.Host.UseSerilog(
            (context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("env", environment)
                .Enrich.WithProperty("service", service)
                .Enrich.WithProperty("version", version)
                .WriteTo.Console(new JsonFormatter(renderMessage: true)));

        return builder;
    }

    private static string GetEnvironmentVariableOrDefault(string name, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name);

        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }
}

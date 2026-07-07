namespace OficinaMecanica.API.Extensions.Security;

public static class WebhookTokenAuthenticationExtensions
{
    public static IServiceCollection AddWebhookTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new WebhookTokenOptions
        {
            WebhookToken = configuration[$"{WebhookTokenOptions.SectionName}:WebhookToken"] ?? string.Empty
        };

        options.Validar();

        services
            .AddOptions<WebhookTokenOptions>()
            .Bind(configuration.GetSection(WebhookTokenOptions.SectionName));

        return services;
    }
}

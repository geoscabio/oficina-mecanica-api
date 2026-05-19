using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.API.Extensions;
using OficinaMecanica.Application.Common;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => options.AddJwtSwagger());
        services.AddJwtAuthentication(configuration);

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = _ =>
                new BadRequestObjectResult(new ErrorResponse(
                    ValidationErrorMessages.RequestInvalido,
                    TipoErro.Validacao));
        });

        return services;
    }
}

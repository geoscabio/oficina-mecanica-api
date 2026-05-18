using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Identidade.Interfaces;
using OficinaMecanica.Infrastructure.Identidade.Services;

namespace OficinaMecanica.Infrastructure.Identidade;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentidadeInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioAutenticadoService, UsuarioAutenticadoService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}

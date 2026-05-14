using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Identidade.UsuarioUseCases;

namespace OficinaMecanica.Application.Identidade;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentidadeApplication(this IServiceCollection services)
    {
        services.AddUsuarioUseCases();

        return services;
    }
}
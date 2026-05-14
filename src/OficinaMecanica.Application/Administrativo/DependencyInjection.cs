using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases;

namespace OficinaMecanica.Application.Administrativo;

public static class DependencyInjection
{
    public static IServiceCollection AddAdministrativoApplication(this IServiceCollection services)
    {
        services.AddServicoCatalogoUseCases();
        services.AddPecaInsumoCatalogoUseCases();

        return services;
    }
}

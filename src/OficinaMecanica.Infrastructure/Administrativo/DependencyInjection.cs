using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Infrastructure.Administrativo.Repositories;

namespace OficinaMecanica.Infrastructure.Administrativo;

public static class DependencyInjection
{
    public static IServiceCollection AddAdministrativoInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMecanicoRepository, MecanicoRepository>();
        services.AddScoped<IServicoCatalogoRepository, ServicoCatalogoRepository>();
        services.AddScoped<IPecaInsumoCatalogoRepository, PecaInsumoCatalogoRepository>();

        return services;
    }
}

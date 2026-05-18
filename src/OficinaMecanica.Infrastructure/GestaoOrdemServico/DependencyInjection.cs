using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Infrastructure.GestaoOrdemServico.Repositories;

namespace OficinaMecanica.Infrastructure.GestaoOrdemServico;

public static class DependencyInjection
{
    public static IServiceCollection AddGestaoOrdemServicoInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();

        return services;
    }
}

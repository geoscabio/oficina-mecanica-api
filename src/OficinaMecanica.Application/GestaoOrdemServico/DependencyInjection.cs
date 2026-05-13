using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases;

namespace OficinaMecanica.Application.GestaoOrdemServico;

public static class DependencyInjection
{
    public static IServiceCollection AddGestaoOrdemServicoApplication(this IServiceCollection services)
    {
        services.AddOrdemServicoUseCases();

        return services;
    }
}

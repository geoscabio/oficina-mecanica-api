using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases;

namespace OficinaMecanica.Application.GestaoEstoque;

public static class DependencyInjection
{
    public static IServiceCollection AddGestaoEstoqueApplication(this IServiceCollection services)
    {
        services.AddEstoqueUseCases();

        return services;
    }
}
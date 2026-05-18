using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Infrastructure.GestaoEstoque.Repositories;

namespace OficinaMecanica.Infrastructure.GestaoEstoque;

public static class DependencyInjection
{
    public static IServiceCollection AddGestaoEstoqueInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IEstoqueRepository, EstoqueRepository>();

        return services;
    }
}

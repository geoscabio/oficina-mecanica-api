using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Infrastructure.Atendimento.Repositories;

namespace OficinaMecanica.Infrastructure.Atendimento;

public static class DependencyInjection
{
    public static IServiceCollection AddAtendimentoInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();

        return services;
    }
}

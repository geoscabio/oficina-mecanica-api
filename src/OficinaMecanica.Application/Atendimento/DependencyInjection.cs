using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Atendimento.ClienteUseCases;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases;

namespace OficinaMecanica.Application.Atendimento;

public static class DependencyInjection
{
    public static IServiceCollection AddAtendimentoApplication(this IServiceCollection services)
    {
        services.AddClienteUseCases();
        services.AddVeiculoUseCases();

        return services;
    }
}

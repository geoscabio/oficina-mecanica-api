using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;

namespace OficinaMecanica.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CadastrarClienteValidator>();
        services.AddScoped<CadastrarClienteUseCase>();
        services.AddScoped<CadastrarVeiculoValidator>();
        services.AddScoped<CadastrarVeiculoUseCase>();

        return services;
    }
}

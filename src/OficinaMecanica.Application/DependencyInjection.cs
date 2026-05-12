using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;

namespace OficinaMecanica.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CadastrarClienteValidator>();
        services.AddScoped<CadastrarClienteUseCase>();
        services.AddScoped<ConsultarClienteValidator>();
        services.AddScoped<ConsultarClienteUseCase>();
        services.AddScoped<CadastrarVeiculoValidator>();
        services.AddScoped<CadastrarVeiculoUseCase>();
        services.AddScoped<ConsultarVeiculoValidator>();
        services.AddScoped<ConsultarVeiculoUseCase>();

        return services;
    }
}

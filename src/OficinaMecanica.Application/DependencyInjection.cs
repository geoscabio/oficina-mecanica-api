using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;

namespace OficinaMecanica.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(_ => { }, typeof(DependencyInjection).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<CadastrarClienteUseCase>();
        services.AddScoped<ConsultarClienteUseCase>();
        services.AddScoped<ConsultarClientePorDocumentoUseCase>();
        services.AddScoped<CadastrarVeiculoUseCase>();
        services.AddScoped<ConsultarVeiculoUseCase>();
        services.AddScoped<ConsultarVeiculoPorPlacaUseCase>();

        return services;
    }
}

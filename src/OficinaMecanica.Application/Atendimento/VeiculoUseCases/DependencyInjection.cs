using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.AtualizarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ListarVeiculos;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddVeiculoUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CadastrarVeiculoRequest>, CadastrarVeiculoValidator>();
        services.AddScoped<IValidator<ConsultarVeiculoRequest>, ConsultarVeiculoValidator>();
        services.AddScoped<IValidator<ConsultarVeiculoPorPlacaRequest>, ConsultarVeiculoPorPlacaValidator>();
        services.AddScoped<IValidator<ListarVeiculosRequest>, ListarVeiculosValidator>();
        services.AddScoped<IValidator<AtualizarVeiculoRequest>, AtualizarVeiculoValidator>();

        services.AddScoped<CadastrarVeiculoUseCase>();
        services.AddScoped<ConsultarVeiculoUseCase>();
        services.AddScoped<ConsultarVeiculoPorPlacaUseCase>();
        services.AddScoped<ListarVeiculosUseCase>();
        services.AddScoped<AtualizarVeiculoUseCase>();

        return services;
    }
}

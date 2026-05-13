using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;

namespace OficinaMecanica.Application.Atendimento;

public static class DependencyInjection
{
    public static IServiceCollection AddAtendimentoApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CadastrarClienteRequest>, CadastrarClienteValidator>();
        services.AddScoped<IValidator<ConsultarClienteRequest>, ConsultarClienteValidator>();
        services.AddScoped<IValidator<ConsultarClientePorDocumentoRequest>, ConsultarClientePorDocumentoValidator>();
        services.AddScoped<IValidator<CadastrarVeiculoRequest>, CadastrarVeiculoValidator>();
        services.AddScoped<IValidator<ConsultarVeiculoRequest>, ConsultarVeiculoValidator>();
        services.AddScoped<IValidator<ConsultarVeiculoPorPlacaRequest>, ConsultarVeiculoPorPlacaValidator>();

        services.AddScoped<CadastrarClienteUseCase>();
        services.AddScoped<ConsultarClienteUseCase>();
        services.AddScoped<ConsultarClientePorDocumentoUseCase>();
        services.AddScoped<CadastrarVeiculoUseCase>();
        services.AddScoped<ConsultarVeiculoUseCase>();
        services.AddScoped<ConsultarVeiculoPorPlacaUseCase>();

        return services;
    }
}

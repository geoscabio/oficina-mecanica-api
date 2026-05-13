using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddClienteUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CadastrarClienteRequest>, CadastrarClienteValidator>();
        services.AddScoped<IValidator<ConsultarClienteRequest>, ConsultarClienteValidator>();
        services.AddScoped<IValidator<ConsultarClientePorDocumentoRequest>, ConsultarClientePorDocumentoValidator>();

        services.AddScoped<CadastrarClienteUseCase>();
        services.AddScoped<ConsultarClienteUseCase>();
        services.AddScoped<ConsultarClientePorDocumentoUseCase>();

        return services;
    }
}

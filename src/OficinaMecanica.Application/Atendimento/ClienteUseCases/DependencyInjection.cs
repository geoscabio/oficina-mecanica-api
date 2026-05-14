using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.AtualizarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ListarClientes;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.RemoverCliente;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddClienteUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CadastrarClienteRequest>, CadastrarClienteValidator>();
        services.AddScoped<IValidator<ConsultarClienteRequest>, ConsultarClienteValidator>();
        services.AddScoped<IValidator<ConsultarClientePorDocumentoRequest>, ConsultarClientePorDocumentoValidator>();
        services.AddScoped<IValidator<ListarClientesRequest>, ListarClientesValidator>();
        services.AddScoped<IValidator<AtualizarClienteRequest>, AtualizarClienteValidator>();
        services.AddScoped<IValidator<RemoverClienteRequest>, RemoverClienteValidator>();

        services.AddScoped<CadastrarClienteUseCase>();
        services.AddScoped<ConsultarClienteUseCase>();
        services.AddScoped<ConsultarClientePorDocumentoUseCase>();
        services.AddScoped<ListarClientesUseCase>();
        services.AddScoped<AtualizarClienteUseCase>();
        services.AddScoped<RemoverClienteUseCase>();

        return services;
    }
}

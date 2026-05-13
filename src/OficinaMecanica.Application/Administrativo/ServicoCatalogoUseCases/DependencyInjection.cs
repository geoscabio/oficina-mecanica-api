using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.CadastrarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarTempoMedioExecucaoServico;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarServicosCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarTempoMedioExecucaoServicos;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddServicoCatalogoUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CadastrarServicoCatalogoRequest>, CadastrarServicoCatalogoValidator>();
        services.AddScoped<IValidator<ConsultarServicoCatalogoRequest>, ConsultarServicoCatalogoValidator>();
        services.AddScoped<IValidator<ListarServicosCatalogoRequest>, ListarServicosCatalogoValidator>();
        services.AddScoped<IValidator<ConsultarTempoMedioExecucaoServicoRequest>, ConsultarTempoMedioExecucaoServicoValidator>();
        services.AddScoped<IValidator<ListarTempoMedioExecucaoServicosRequest>, ListarTempoMedioExecucaoServicosValidator>();

        services.AddScoped<CadastrarServicoCatalogoUseCase>();
        services.AddScoped<ConsultarServicoCatalogoUseCase>();
        services.AddScoped<ListarServicosCatalogoUseCase>();
        services.AddScoped<ConsultarTempoMedioExecucaoServicoUseCase>();
        services.AddScoped<ListarTempoMedioExecucaoServicosUseCase>();

        return services;
    }
}

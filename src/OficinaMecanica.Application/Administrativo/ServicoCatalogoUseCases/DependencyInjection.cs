using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarTempoMedioExecucaoServico;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarTempoMedioExecucaoServicos;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddServicoCatalogoUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<ConsultarTempoMedioExecucaoServicoRequest>, ConsultarTempoMedioExecucaoServicoValidator>();
        services.AddScoped<IValidator<ListarTempoMedioExecucaoServicosRequest>, ListarTempoMedioExecucaoServicosValidator>();

        services.AddScoped<ConsultarTempoMedioExecucaoServicoUseCase>();
        services.AddScoped<ListarTempoMedioExecucaoServicosUseCase>();

        return services;
    }
}

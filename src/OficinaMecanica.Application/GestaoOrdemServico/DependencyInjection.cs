using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DefinirServicos;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;

namespace OficinaMecanica.Application.GestaoOrdemServico;

public static class DependencyInjection
{
    public static IServiceCollection AddGestaoOrdemServicoApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AbrirOrdemServicoRequest>, AbrirOrdemServicoValidator>();
        services.AddScoped<IValidator<IniciarDiagnosticoOrdemServicoRequest>, IniciarDiagnosticoOrdemServicoValidator>();
        services.AddScoped<IValidator<DefinirServicosRequest>, DefinirServicosValidator>();

        services.AddScoped<AbrirOrdemServicoUseCase>();
        services.AddScoped<IniciarDiagnosticoOrdemServicoUseCase>();
        services.AddScoped<DefinirServicosUseCase>();

        return services;
    }
}

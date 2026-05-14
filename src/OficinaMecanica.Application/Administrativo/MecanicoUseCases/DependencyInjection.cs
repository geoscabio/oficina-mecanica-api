using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.AtualizarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.CadastrarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ListarMecanicos;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddMecanicoUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CadastrarMecanicoRequest>, CadastrarMecanicoValidator>();
        services.AddScoped<IValidator<ListarMecanicosRequest>, ListarMecanicosValidator>();
        services.AddScoped<IValidator<AtualizarMecanicoRequest>, AtualizarMecanicoValidator>();

        services.AddScoped<CadastrarMecanicoUseCase>();
        services.AddScoped<ListarMecanicosUseCase>();
        services.AddScoped<AtualizarMecanicoUseCase>();

        return services;
    }
}
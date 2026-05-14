using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.AtualizarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.CadastrarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ConsultarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ListarMecanicos;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.RemoverMecanico;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddMecanicoUseCases(this IServiceCollection services)
    {
        services.AddScoped<CadastrarMecanicoUseCase>();
        services.AddScoped<IValidator<CadastrarMecanicoRequest>, CadastrarMecanicoValidator>();

        services.AddScoped<ListarMecanicosUseCase>();
        services.AddScoped<IValidator<ListarMecanicosRequest>, ListarMecanicosValidator>();

        services.AddScoped<AtualizarMecanicoUseCase>();
        services.AddScoped<IValidator<AtualizarMecanicoRequest>, AtualizarMecanicoValidator>();

        services.AddScoped<RemoverMecanicoUseCase>();
        services.AddScoped<IValidator<RemoverMecanicoRequest>, RemoverMecanicoValidator>();

        services.AddScoped<ConsultarMecanicoUseCase>();
        services.AddScoped<IValidator<ConsultarMecanicoRequest>, ConsultarMecanicoValidator>();


        return services;
    }
}
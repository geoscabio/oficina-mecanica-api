using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.CadastrarMecanico;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddMecanicoUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CadastrarMecanicoRequest>, CadastrarMecanicoValidator>();

        services.AddScoped<CadastrarMecanicoUseCase>();

        return services;
    }
}
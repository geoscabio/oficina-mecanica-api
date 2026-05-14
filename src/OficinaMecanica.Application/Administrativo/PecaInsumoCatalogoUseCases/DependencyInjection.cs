using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddPecaInsumoCatalogoUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CadastrarPecaInsumoCatalogoRequest>, CadastrarPecaInsumoCatalogoValidator>();

        services.AddScoped<CadastrarPecaInsumoCatalogoUseCase>();

        return services;
    }
}
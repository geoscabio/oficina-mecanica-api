using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ListarPecasInsumosCatalogo;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddPecaInsumoCatalogoUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CadastrarPecaInsumoCatalogoRequest>, CadastrarPecaInsumoCatalogoValidator>();
        services.AddScoped<IValidator<ListarPecasInsumosCatalogoRequest>, ListarPecasInsumosCatalogoValidator>();

        services.AddScoped<CadastrarPecaInsumoCatalogoUseCase>();
        services.AddScoped<ListarPecasInsumosCatalogoUseCase>();

        return services;
    }
}
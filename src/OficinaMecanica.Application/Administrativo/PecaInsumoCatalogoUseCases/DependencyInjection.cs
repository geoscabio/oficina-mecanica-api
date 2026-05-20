using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.AtualizarPecaInsumoCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ConsultarPecaInsumoCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ListarPecasInsumosCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.RemoverPecaInsumoCatalogo;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddPecaInsumoCatalogoUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CadastrarPecaInsumoCatalogoRequest>, CadastrarPecaInsumoCatalogoValidator>();
        services.AddScoped<IValidator<ListarPecasInsumosCatalogoRequest>, ListarPecasInsumosCatalogoValidator>();
        services.AddScoped<IValidator<AtualizarPecaInsumoCatalogoRequest>, AtualizarPecaInsumoCatalogoValidator>();
        services.AddScoped<IValidator<RemoverPecaInsumoCatalogoRequest>, RemoverPecaInsumoCatalogoValidator>();
        services.AddScoped<IValidator<ConsultarPecaInsumoCatalogoRequest>, ConsultarPecaInsumoCatalogoValidator>();

        services.AddScoped<CadastrarPecaInsumoCatalogoUseCase>();
        services.AddScoped<ListarPecasInsumosCatalogoUseCase>();
        services.AddScoped<AtualizarPecaInsumoCatalogoUseCase>();
        services.AddScoped<RemoverPecaInsumoCatalogoUseCase>();
        services.AddScoped<ConsultarPecaInsumoCatalogoUseCase>();

        return services;
    }
}
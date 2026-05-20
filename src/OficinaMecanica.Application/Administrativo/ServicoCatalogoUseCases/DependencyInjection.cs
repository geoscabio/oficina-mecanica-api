using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.AtualizarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.CadastrarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarServicosCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.RemoverServicoCatalogo;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddServicoCatalogoUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AtualizarServicoCatalogoRequest>, AtualizarServicoCatalogoValidator>();
        services.AddScoped<IValidator<CadastrarServicoCatalogoRequest>, CadastrarServicoCatalogoValidator>();
        services.AddScoped<IValidator<ConsultarServicoCatalogoRequest>, ConsultarServicoCatalogoValidator>();
        services.AddScoped<IValidator<ListarServicosCatalogoRequest>, ListarServicosCatalogoValidator>();
        services.AddScoped<IValidator<RemoverServicoCatalogoRequest>, RemoverServicoCatalogoValidator>();

        services.AddScoped<AtualizarServicoCatalogoUseCase>();
        services.AddScoped<CadastrarServicoCatalogoUseCase>();
        services.AddScoped<ConsultarServicoCatalogoUseCase>();
        services.AddScoped<ListarServicosCatalogoUseCase>();
        services.AddScoped<RemoverServicoCatalogoUseCase>();

        return services;
    }
}

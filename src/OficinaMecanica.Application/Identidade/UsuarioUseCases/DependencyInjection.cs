using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;

namespace OficinaMecanica.Application.Identidade.UsuarioUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddUsuarioUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AutenticarUsuarioRequest>, AutenticarUsuarioValidator>();

        services.AddScoped<AutenticarUsuarioUseCase>();

        return services;
    }
}
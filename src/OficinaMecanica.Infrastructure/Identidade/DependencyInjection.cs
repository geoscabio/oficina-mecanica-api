using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Identidade.Interfaces;
using OficinaMecanica.Infrastructure.Identidade.Options;
using OficinaMecanica.Infrastructure.Identidade.Services;

namespace OficinaMecanica.Infrastructure.Identidade;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentidadeInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => options.EhValido(), "Configuracao JWT invalida.")
            .ValidateOnStart();

        services
            .AddOptions<IdentidadeOptions>()
            .Bind(configuration.GetSection(IdentidadeOptions.SectionName))
            .Validate(options => options.UsuariosDemo.Any(usuario => usuario.EhValido()), "Configure ao menos um usuario demo valido para autenticacao do MVP.")
            .ValidateOnStart();

        services.AddScoped<IUsuarioAutenticadoService, UsuarioDemoAutenticadoService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}

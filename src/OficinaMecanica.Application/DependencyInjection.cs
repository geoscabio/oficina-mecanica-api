using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.Administrativo;
using OficinaMecanica.Application.Atendimento;
using OficinaMecanica.Application.GestaoOrdemServico;

namespace OficinaMecanica.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(_ => { }, typeof(DependencyInjection).Assembly);

        services.AddAdministrativoApplication();
        services.AddAtendimentoApplication();
        services.AddGestaoOrdemServicoApplication();

        return services;
    }
}

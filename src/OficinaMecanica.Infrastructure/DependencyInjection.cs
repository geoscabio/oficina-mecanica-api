using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Infrastructure.Administrativo;
using OficinaMecanica.Infrastructure.Atendimento;
using OficinaMecanica.Infrastructure.GestaoEstoque;
using OficinaMecanica.Infrastructure.GestaoOrdemServico;
using OficinaMecanica.Infrastructure.Identidade;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' nao configurada.");

        services.AddDbContext<OficinaMecanicaDbContext>(options => options.UseSqlServer(connectionString));
        services.AddAdministrativoInfrastructure();
        services.AddAtendimentoInfrastructure();
        services.AddGestaoEstoqueInfrastructure();
        services.AddGestaoOrdemServicoInfrastructure();
        services.AddIdentidadeInfrastructure(configuration);

        return services;
    }
}

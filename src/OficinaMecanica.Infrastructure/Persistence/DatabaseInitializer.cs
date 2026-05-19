using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OficinaMecanica.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider serviceProvider,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<OficinaMecanicaDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("OficinaMecanica.Database");

        if (IsEnabled(configuration, "Database:ApplyMigrationsOnStartup"))
        {
            logger.LogInformation("Aplicando migrations do banco de dados...");
            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Banco de dados atualizado.");
        }

        if (IsEnabled(configuration, "Database:SeedDemoData"))
        {
            logger.LogInformation("Carregando dados demo...");
            await DemoDatabaseSeeder.SeedAsync(dbContext, cancellationToken);
            logger.LogInformation("Dados demo prontos.");
        }
    }

    private static bool IsEnabled(IConfiguration configuration, string key)
    {
        return !bool.TryParse(configuration[key], out var enabled) || enabled;
    }
}

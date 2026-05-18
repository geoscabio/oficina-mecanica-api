using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        if (IsEnabled(configuration, "Database:ApplyMigrationsOnStartup"))
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        if (IsEnabled(configuration, "Database:SeedDemoData"))
        {
            await DemoDatabaseSeeder.SeedAsync(dbContext, cancellationToken);
        }
    }

    private static bool IsEnabled(IConfiguration configuration, string key)
    {
        return !bool.TryParse(configuration[key], out var enabled) || enabled;
    }
}

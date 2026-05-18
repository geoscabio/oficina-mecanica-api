using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OficinaMecanica.Infrastructure.Persistence;

public sealed class DesignTimeOficinaMecanicaDbContextFactory : IDesignTimeDbContextFactory<OficinaMecanicaDbContext>
{
    public OficinaMecanicaDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var apiProjectPath = Directory.Exists(Path.Combine(currentDirectory, "src", "OficinaMecanica.API"))
            ? Path.Combine(currentDirectory, "src", "OficinaMecanica.API")
            : Path.Combine(currentDirectory, "..", "OficinaMecanica.API");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetFullPath(apiProjectPath))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' nao configurada.");

        var optionsBuilder = new DbContextOptionsBuilder<OficinaMecanicaDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new OficinaMecanicaDbContext(optionsBuilder.Options);
    }
}

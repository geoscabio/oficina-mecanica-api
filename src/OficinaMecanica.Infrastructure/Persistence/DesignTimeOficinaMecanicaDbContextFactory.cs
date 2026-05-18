using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OficinaMecanica.Infrastructure.Persistence;

public sealed class DesignTimeOficinaMecanicaDbContextFactory : IDesignTimeDbContextFactory<OficinaMecanicaDbContext>
{
    private const string ConnectionStringName = "DefaultConnection";

    public OficinaMecanicaDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(ObterCaminhoProjetoApi())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' nao configurada.");

        var optionsBuilder = new DbContextOptionsBuilder<OficinaMecanicaDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new OficinaMecanicaDbContext(optionsBuilder.Options);
    }

    private static string ObterCaminhoProjetoApi()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var caminhosCandidatos = new[]
        {
            currentDirectory,
            Path.Combine(currentDirectory, "src", "OficinaMecanica.API"),
            Path.Combine(currentDirectory, "..", "OficinaMecanica.API")
        };

        foreach (var caminho in caminhosCandidatos)
        {
            var caminhoCompleto = Path.GetFullPath(caminho);

            if (File.Exists(Path.Combine(caminhoCompleto, "appsettings.json")))
            {
                return caminhoCompleto;
            }
        }

        throw new InvalidOperationException("Projeto OficinaMecanica.API nao encontrado.");
    }
}

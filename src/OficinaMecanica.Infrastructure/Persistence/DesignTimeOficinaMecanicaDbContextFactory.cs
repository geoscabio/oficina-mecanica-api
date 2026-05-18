using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace OficinaMecanica.Infrastructure.Persistence;

public sealed class DesignTimeOficinaMecanicaDbContextFactory : IDesignTimeDbContextFactory<OficinaMecanicaDbContext>
{
    private const string ConnectionStringName = "DefaultConnection";

    public OficinaMecanicaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OficinaMecanicaDbContext>();
        var connectionString = ObterConnectionString()
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' nao configurada.");

        optionsBuilder.UseSqlServer(connectionString);

        return new OficinaMecanicaDbContext(optionsBuilder.Options);
    }

    private static string? ObterConnectionString()
    {
        return Environment.GetEnvironmentVariable($"ConnectionStrings__{ConnectionStringName}")
            ?? LerConnectionStringDeAppsettings("appsettings.Development.json")
            ?? LerConnectionStringDeAppsettings("appsettings.json");
    }

    private static string? LerConnectionStringDeAppsettings(string fileName)
    {
        foreach (var directory in ObterDiretoriosCandidatos())
        {
            var path = Path.Combine(directory, fileName);

            if (!File.Exists(path))
            {
                continue;
            }

            using var stream = File.OpenRead(path);
            using var document = JsonDocument.Parse(stream);

            if (document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings)
                && connectionStrings.TryGetProperty(ConnectionStringName, out var connectionString))
            {
                return connectionString.GetString();
            }
        }

        return null;
    }

    private static IEnumerable<string> ObterDiretoriosCandidatos()
    {
        var directories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (currentDirectory is not null)
        {
            AdicionarSeExistir(directories, currentDirectory.FullName);
            AdicionarSeExistir(directories, Path.Combine(currentDirectory.FullName, "src", "OficinaMecanica.API"));
            AdicionarSeExistir(directories, Path.Combine(currentDirectory.FullName, "..", "OficinaMecanica.API"));

            currentDirectory = currentDirectory.Parent;
        }

        return directories;
    }

    private static void AdicionarSeExistir(HashSet<string> directories, string path)
    {
        var fullPath = Path.GetFullPath(path);

        if (Directory.Exists(fullPath))
        {
            directories.Add(fullPath);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OficinaMecanica.Infrastructure.Persistence;

public sealed class DesignTimeOficinaMecanicaDbContextFactory : IDesignTimeDbContextFactory<OficinaMecanicaDbContext>
{
    public OficinaMecanicaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OficinaMecanicaDbContext>();

        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OficinaMecanicaDb;Trusted_Connection=True;TrustServerCertificate=True;");

        return new OficinaMecanicaDbContext(optionsBuilder.Options);
    }
}

using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

namespace OficinaMecanica.Infrastructure.Persistence;

public sealed class OficinaMecanicaDbContext : DbContext
{
    public OficinaMecanicaDbContext(DbContextOptions<OficinaMecanicaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Mecanico> Mecanicos => Set<Mecanico>();
    public DbSet<ServicoCatalogo> ServicosCatalogo => Set<ServicoCatalogo>();
    public DbSet<PecaInsumoCatalogo> PecasInsumosCatalogo => Set<PecaInsumoCatalogo>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();
    public DbSet<Estoque> Estoques => Set<Estoque>();
    public DbSet<OrdemServico> OrdensServico => Set<OrdemServico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OficinaMecanicaDbContext).Assembly);
    }
}

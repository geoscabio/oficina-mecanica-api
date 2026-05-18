using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Entities;

namespace OficinaMecanica.Infrastructure.GestaoOrdemServico.Configurations;

public sealed class PecaInsumoConfiguration : IEntityTypeConfiguration<PecaInsumo>
{
    public void Configure(EntityTypeBuilder<PecaInsumo> builder)
    {
        builder.ToTable("PecasInsumosOrdemServico", "GestaoOrdemServico");

        builder.HasKey(pecaInsumo => pecaInsumo.Id);

        builder.Property(pecaInsumo => pecaInsumo.Id)
            .ValueGeneratedNever();

        builder.Property<Guid>("OrdemServicoId")
            .IsRequired();

        builder.Property(pecaInsumo => pecaInsumo.PecaInsumoCatalogoId)
            .IsRequired();

        builder.Property(pecaInsumo => pecaInsumo.Quantidade)
            .IsRequired();

        builder.Property(pecaInsumo => pecaInsumo.ValorUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Ignore(pecaInsumo => pecaInsumo.ValorTotal);

        builder.HasIndex("OrdemServicoId");
        builder.HasIndex(pecaInsumo => pecaInsumo.PecaInsumoCatalogoId);

        builder.HasOne<PecaInsumoCatalogo>()
            .WithMany()
            .HasForeignKey(pecaInsumo => pecaInsumo.PecaInsumoCatalogoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

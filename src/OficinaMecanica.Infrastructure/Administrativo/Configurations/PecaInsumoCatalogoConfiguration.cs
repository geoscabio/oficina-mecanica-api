using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;

namespace OficinaMecanica.Infrastructure.Administrativo.Configurations;

public sealed class PecaInsumoCatalogoConfiguration : IEntityTypeConfiguration<PecaInsumoCatalogo>
{
    public void Configure(EntityTypeBuilder<PecaInsumoCatalogo> builder)
    {
        builder.ToTable("PecasInsumosCatalogo", "Administrativo");

        builder.HasKey(pecaInsumo => pecaInsumo.Id);

        builder.Property(pecaInsumo => pecaInsumo.Id)
            .ValueGeneratedNever();

        builder.Property(pecaInsumo => pecaInsumo.Descricao)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(pecaInsumo => pecaInsumo.Tipo)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(pecaInsumo => pecaInsumo.Valor)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Infrastructure.Administrativo.Configurations;

public sealed class ServicoCatalogoConfiguration : IEntityTypeConfiguration<ServicoCatalogo>
{
    public void Configure(EntityTypeBuilder<ServicoCatalogo> builder)
    {
        builder.ToTable("ServicosCatalogo", "Administrativo");

        builder.HasKey(servico => servico.Id);

        builder.Property(servico => servico.Id)
            .ValueGeneratedNever();

        builder.Property(servico => servico.Descricao)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(servico => servico.Valor)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}

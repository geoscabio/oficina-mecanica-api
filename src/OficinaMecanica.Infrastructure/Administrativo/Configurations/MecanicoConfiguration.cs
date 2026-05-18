using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Infrastructure.Administrativo.Configurations;

public sealed class MecanicoConfiguration : IEntityTypeConfiguration<Mecanico>
{
    public void Configure(EntityTypeBuilder<Mecanico> builder)
    {
        builder.ToTable("Mecanicos", "Administrativo");

        builder.HasKey(mecanico => mecanico.Id);

        builder.Property(mecanico => mecanico.Id)
            .ValueGeneratedNever();

        builder.Property(mecanico => mecanico.Nome)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(mecanico => mecanico.Funcional)
            .HasMaxLength(30)
            .IsRequired();
    }
}

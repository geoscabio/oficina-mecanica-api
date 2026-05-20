using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OficinaMecanica.Domain.Atendimento.Aggregates;

namespace OficinaMecanica.Infrastructure.Atendimento.Configurations;

public sealed class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> builder)
    {
        builder.ToTable("Veiculos", "Atendimento");

        builder.HasKey(veiculo => veiculo.Id);

        builder.Property(veiculo => veiculo.Id)
            .ValueGeneratedNever();

        builder.Property(veiculo => veiculo.ClienteId)
            .IsRequired();

        builder.Property(veiculo => veiculo.Marca)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(veiculo => veiculo.Modelo)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(veiculo => veiculo.Ano)
            .IsRequired();

        builder.OwnsOne(veiculo => veiculo.Placa, placa =>
        {
            placa.Property(item => item.NumeroPlaca)
                .HasColumnName("Placa")
                .HasMaxLength(7)
                .IsRequired();

            placa.HasIndex(item => item.NumeroPlaca)
                .IsUnique();
        });

        builder.Navigation(veiculo => veiculo.Placa).IsRequired();

        builder.HasIndex(veiculo => veiculo.ClienteId);

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(veiculo => veiculo.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

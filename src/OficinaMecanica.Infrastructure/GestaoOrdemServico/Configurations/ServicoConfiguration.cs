using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Entities;

namespace OficinaMecanica.Infrastructure.GestaoOrdemServico.Configurations;

public sealed class ServicoConfiguration : IEntityTypeConfiguration<Servico>
{
    public void Configure(EntityTypeBuilder<Servico> builder)
    {
        builder.ToTable("ServicosOrdemServico", "GestaoOrdemServico");

        builder.HasKey(servico => servico.Id);

        builder.Property(servico => servico.Id)
            .ValueGeneratedNever();

        builder.Property<Guid>("OrdemServicoId")
            .IsRequired();

        builder.Property(servico => servico.DataInicio);
        builder.Property(servico => servico.DataFim);

        builder.Property(servico => servico.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(servico => servico.ServicoCatalogoId)
            .IsRequired();

        builder.Property(servico => servico.Valor)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasIndex("OrdemServicoId");
        builder.HasIndex(servico => servico.ServicoCatalogoId);

        builder.HasOne<ServicoCatalogo>()
            .WithMany()
            .HasForeignKey(servico => servico.ServicoCatalogoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

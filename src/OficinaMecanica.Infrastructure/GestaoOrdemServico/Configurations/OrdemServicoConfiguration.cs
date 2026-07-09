using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

namespace OficinaMecanica.Infrastructure.GestaoOrdemServico.Configurations;

public sealed class OrdemServicoConfiguration : IEntityTypeConfiguration<OrdemServico>
{
    public void Configure(EntityTypeBuilder<OrdemServico> builder)
    {
        builder.ToTable("OrdensServico", "GestaoOrdemServico");

        builder.HasKey(ordemServico => ordemServico.Id);

        builder.Property(ordemServico => ordemServico.Id)
            .ValueGeneratedNever();

        builder.Property(ordemServico => ordemServico.Numero)
            .IsRequired();

        builder.Property(ordemServico => ordemServico.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(ordemServico => ordemServico.ValorTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(ordemServico => ordemServico.DataInicio)
            .IsRequired();
        builder.Property(ordemServico => ordemServico.DataFim);

        builder.Property(ordemServico => ordemServico.MotivoCancelamento)
            .HasConversion<int?>();

        builder.Property(ordemServico => ordemServico.VeiculoId)
            .IsRequired();

        builder.Property(ordemServico => ordemServico.MecanicoId)
            .IsRequired();

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();

        builder.HasMany(ordemServico => ordemServico.Servicos)
            .WithOne()
            .HasForeignKey("OrdemServicoId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ordemServico => ordemServico.PecasInsumos)
            .WithOne()
            .HasForeignKey("OrdemServicoId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(ordemServico => ordemServico.Servicos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(ordemServico => ordemServico.PecasInsumos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(ordemServico => ordemServico.Numero)
            .IsUnique();

        builder.HasIndex(ordemServico => ordemServico.VeiculoId);
        builder.HasIndex(ordemServico => ordemServico.MecanicoId);

        builder.HasOne<Veiculo>()
            .WithMany()
            .HasForeignKey(ordemServico => ordemServico.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Mecanico>()
            .WithMany()
            .HasForeignKey(ordemServico => ordemServico.MecanicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

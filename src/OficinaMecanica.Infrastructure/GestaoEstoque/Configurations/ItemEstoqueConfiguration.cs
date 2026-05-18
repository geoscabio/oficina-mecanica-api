using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;

namespace OficinaMecanica.Infrastructure.GestaoEstoque.Configurations;

public sealed class ItemEstoqueConfiguration : IEntityTypeConfiguration<ItemEstoque>
{
    public void Configure(EntityTypeBuilder<ItemEstoque> builder)
    {
        builder.ToTable("ItensEstoque", "GestaoEstoque");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .ValueGeneratedNever();

        builder.Property<Guid>("EstoqueId")
            .IsRequired();

        builder.Property(item => item.PecaInsumoCatalogoId)
            .IsRequired();

        builder.Property(item => item.QuantidadeDisponivel)
            .IsRequired();

        builder.Property(item => item.QuantidadeReservada)
            .IsRequired();

        builder.HasIndex(item => item.PecaInsumoCatalogoId)
            .IsUnique();

        builder.HasOne<PecaInsumoCatalogo>()
            .WithMany()
            .HasForeignKey(item => item.PecaInsumoCatalogoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;

namespace OficinaMecanica.Infrastructure.GestaoEstoque.Configurations;

public sealed class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
{
    public void Configure(EntityTypeBuilder<Estoque> builder)
    {
        builder.ToTable("Estoques", "GestaoEstoque");

        builder.HasKey(estoque => estoque.Id);

        builder.Property(estoque => estoque.Id)
            .ValueGeneratedNever();

        builder.HasMany(estoque => estoque.ItensEstoque)
            .WithOne()
            .HasForeignKey("EstoqueId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(estoque => estoque.ItensEstoque)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

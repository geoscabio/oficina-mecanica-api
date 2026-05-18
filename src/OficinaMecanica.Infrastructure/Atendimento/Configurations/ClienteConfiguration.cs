using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OficinaMecanica.Domain.Atendimento.Aggregates;

namespace OficinaMecanica.Infrastructure.Atendimento.Configurations;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes", "Atendimento");

        builder.HasKey(cliente => cliente.Id);

        builder.Property(cliente => cliente.Id)
            .ValueGeneratedNever();

        builder.Property(cliente => cliente.Nome)
            .HasMaxLength(160)
            .IsRequired();

        builder.OwnsOne(cliente => cliente.Documento, documento =>
        {
            documento.Property(item => item.Numero)
                .HasColumnName("Documento")
                .HasMaxLength(14)
                .IsRequired();

            documento.Property(item => item.Tipo)
                .HasColumnName("TipoDocumento")
                .HasConversion<int>()
                .IsRequired();

            documento.HasIndex(item => item.Numero)
                .IsUnique();
        });

        builder.OwnsOne(cliente => cliente.Endereco, endereco =>
        {
            endereco.Property(item => item.Logradouro)
                .HasColumnName("EnderecoLogradouro")
                .HasMaxLength(160)
                .IsRequired();

            endereco.Property(item => item.Numero)
                .HasColumnName("EnderecoNumero")
                .HasMaxLength(20)
                .IsRequired();

            endereco.Property(item => item.Bairro)
                .HasColumnName("EnderecoBairro")
                .HasMaxLength(120)
                .IsRequired();

            endereco.Property(item => item.Cidade)
                .HasColumnName("EnderecoCidade")
                .HasMaxLength(120)
                .IsRequired();

            endereco.Property(item => item.CEP)
                .HasColumnName("EnderecoCEP")
                .HasMaxLength(8)
                .IsRequired();
        });

        builder.OwnsOne(cliente => cliente.Telefone, telefone =>
        {
            telefone.Property(item => item.Numero)
                .HasColumnName("Telefone")
                .HasMaxLength(11)
                .IsRequired();
        });

        builder.OwnsOne(cliente => cliente.Email, email =>
        {
            email.Property(item => item.Endereco)
                .HasColumnName("Email")
                .HasMaxLength(160)
                .IsRequired();
        });

        builder.Navigation(cliente => cliente.Documento).IsRequired();
        builder.Navigation(cliente => cliente.Endereco).IsRequired();
        builder.Navigation(cliente => cliente.Telefone).IsRequired();
        builder.Navigation(cliente => cliente.Email).IsRequired();
    }
}

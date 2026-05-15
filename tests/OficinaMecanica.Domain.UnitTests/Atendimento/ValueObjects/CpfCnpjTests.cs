using FluentAssertions;
using OficinaMecanica.Domain.Atendimento.Enums;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.Atendimento.ValueObjects;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.Atendimento.Factories;

namespace OficinaMecanica.Domain.UnitTests.Atendimento.ValueObjects;

public class CpfCnpjTests
{
    [Fact]
    public void Dado_CpfValidoComMascara_Quando_Criar_Entao_DeveNormalizarNumeroEIdentificarTipoCpf()
    {
        // Arrange
        var cpf = ClienteTestDataFactory.DocumentoPadrao;

        // Act
        var documento = CpfCnpj.Criar(cpf);

        // Assert
        documento.Numero.Should().Be(ClienteTestDataFactory.DocumentoNormalizadoPadrao);
        documento.Tipo.Should().Be(TipoDocumento.CPF);
    }

    [Fact]
    public void Dado_CnpjValidoComMascara_Quando_Criar_Entao_DeveNormalizarNumeroEIdentificarTipoCnpj()
    {
        // Arrange
        var cnpj = ClienteTestDataFactory.CnpjPadrao;

        // Act
        var documento = CpfCnpj.Criar(cnpj);

        // Assert
        documento.Numero.Should().Be(ClienteTestDataFactory.CnpjNormalizadoPadrao);
        documento.Tipo.Should().Be(TipoDocumento.CNPJ);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("111.111.111-11")]
    [InlineData("12.345.678/0001-99")]
    [InlineData("123")]
    public void Dado_DocumentoInvalido_Quando_Criar_Entao_DeveLancarDomainException(string numero)
    {
        // Arrange
        var numeroInformado = numero;

        // Act
        var acao = () => CpfCnpj.Criar(numeroInformado);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(ClienteErrorMessages.DocumentoInvalido);
    }

    [Fact]
    public void Dado_DocumentosComMesmosDigitos_Quando_Comparar_Entao_DevemSerIguaisPorValor()
    {
        // Arrange
        var documentoComMascara = CpfCnpj.Criar(ClienteTestDataFactory.DocumentoPadrao);
        var documentoSemMascara = CpfCnpj.Criar(ClienteTestDataFactory.DocumentoNormalizadoPadrao);

        // Act
        var documentosIguais = documentoComMascara.Equals(documentoSemMascara);

        // Assert
        documentosIguais.Should().BeTrue();
        documentoComMascara.Should().Be(documentoSemMascara);
    }
}
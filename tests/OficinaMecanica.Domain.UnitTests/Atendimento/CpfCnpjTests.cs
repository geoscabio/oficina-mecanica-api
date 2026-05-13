using FluentAssertions;
using OficinaMecanica.Domain.Atendimento.Enums;
using OficinaMecanica.Domain.Atendimento.Exceptions;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.UnitTests.Atendimento;

public class CpfCnpjTests
{
    [Fact]
    public void Dado_CpfValidoComMascara_Quando_Criar_Entao_DeveNormalizarNumeroEIdentificarTipoCpf()
    {
        // Arrange
        const string cpf = "529.982.247-25";

        // Act
        var documento = CpfCnpj.Criar(cpf);

        // Assert
        documento.Numero.Should().Be("52998224725");
        documento.Tipo.Should().Be(TipoDocumento.CPF);
    }

    [Fact]
    public void Dado_CnpjValidoComMascara_Quando_Criar_Entao_DeveNormalizarNumeroEIdentificarTipoCnpj()
    {
        // Arrange
        const string cnpj = "04.252.011/0001-10";

        // Act
        var documento = CpfCnpj.Criar(cnpj);

        // Assert
        documento.Numero.Should().Be("04252011000110");
        documento.Tipo.Should().Be(TipoDocumento.CNPJ);
    }

    [Theory]
    [InlineData("")]
    [InlineData("111.111.111-11")]
    [InlineData("12.345.678/0001-99")]
    [InlineData("123")]
    public void Dado_DocumentoInvalido_Quando_Criar_Entao_DeveLancarDocumentoInvalidoException(string numero)
    {
        // Arrange
        var numeroInformado = numero;

        // Act
        var acao = () => CpfCnpj.Criar(numeroInformado);

        // Assert
        acao.Should().Throw<DocumentoInvalidoException>();
    }

    [Fact]
    public void Dado_DocumentosComMesmosDigitos_Quando_Comparar_Entao_DevemSerIguaisPorValor()
    {
        // Arrange
        var documentoComMascara = CpfCnpj.Criar("529.982.247-25");
        var documentoSemMascara = CpfCnpj.Criar("52998224725");

        // Act
        var documentosIguais = documentoComMascara.Equals(documentoSemMascara);

        // Assert
        documentosIguais.Should().BeTrue();
        documentoComMascara.Should().Be(documentoSemMascara);
    }
}

using FluentAssertions;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.Administrativo.Factories;

namespace OficinaMecanica.Domain.UnitTests.Administrativo;

public class ServicoCatalogoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarServicoCatalogo_Entao_DeveRegistrarServicoComValor()
    {
        // Arrange
        const string descricao = ServicoCatalogoTestDataFactory.DescricaoPadrao;
        const decimal valor = ServicoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var servico = ServicoCatalogo.Criar(
            descricao,
            valor);

        // Assert
        servico.Id.Should().NotBeEmpty();
        servico.Descricao.Should().Be(ServicoCatalogoTestDataFactory.DescricaoPadrao);
        servico.Valor.Should().Be(ServicoCatalogoTestDataFactory.ValorPadrao);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_DescricaoInvalida_Quando_CriarServicoCatalogo_Entao_DeveLancarDomainException(
        string descricao)
    {
        // Arrange
        const decimal valor = ServicoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var acao = () => ServicoCatalogo.Criar(
            descricao,
            valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(ServicoCatalogoErrorMessages.DescricaoObrigatoria);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_CriarServicoCatalogo_Entao_DeveLancarDomainException(
        decimal valor)
    {
        // Arrange
        const string descricao = ServicoCatalogoTestDataFactory.DescricaoPadrao;

        // Act
        var acao = () => ServicoCatalogo.Criar(
            descricao,
            valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(ServicoCatalogoErrorMessages.ValorMaiorQueZero);
    }

    [Fact]
    public void Dado_DadosValidos_Quando_AtualizarServicoCatalogo_Entao_DeveAtualizarDescricaoEValor()
    {
        // Arrange
        var servico = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        // Act
        servico.Atualizar(
            ServicoCatalogoTestDataFactory.DescricaoAtualizada,
            ServicoCatalogoTestDataFactory.ValorAtualizado);

        // Assert
        servico.Descricao.Should().Be(ServicoCatalogoTestDataFactory.DescricaoAtualizada);
        servico.Valor.Should().Be(ServicoCatalogoTestDataFactory.ValorAtualizado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_DescricaoInvalida_Quando_AtualizarServicoCatalogo_Entao_DeveLancarDomainException(
        string descricao)
    {
        // Arrange
        var servico = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        // Act
        var acao = () => servico.Atualizar(
            descricao,
            ServicoCatalogoTestDataFactory.ValorAtualizado);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(ServicoCatalogoErrorMessages.DescricaoObrigatoria);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_AtualizarServicoCatalogo_Entao_DeveLancarDomainException(
        decimal valor)
    {
        // Arrange
        var servico = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        // Act
        var acao = () => servico.Atualizar(
            ServicoCatalogoTestDataFactory.DescricaoAtualizada,
            valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(ServicoCatalogoErrorMessages.ValorMaiorQueZero);
    }
}
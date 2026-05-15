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
        const string descricaoEsperada = ServicoCatalogoTestDataFactory.DescricaoPadrao;
        const decimal valorEsperado = ServicoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var servico = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        // Assert
        servico.Id.Should().NotBeEmpty();
        servico.Descricao.Should().Be(descricaoEsperada);
        servico.Valor.Should().Be(valorEsperado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_DescricaoInvalida_Quando_CriarServicoCatalogo_Entao_DeveLancarDomainException(string descricao)
    {
        // Arrange
        const decimal valor = ServicoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var acao = () => ServicoCatalogo.Criar(descricao, valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(ServicoCatalogoErrorMessages.DescricaoObrigatoria);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_CriarServicoCatalogo_Entao_DeveLancarDomainException(decimal valor)
    {
        // Arrange
        const string descricao = ServicoCatalogoTestDataFactory.DescricaoPadrao;

        // Act
        var acao = () => ServicoCatalogo.Criar(descricao, valor);

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
        const string descricaoAtualizada = "Alinhamento";
        const decimal valorAtualizado = 90m;

        // Act
        servico.Atualizar(descricaoAtualizada, valorAtualizado);

        // Assert
        servico.Descricao.Should().Be(descricaoAtualizada);
        servico.Valor.Should().Be(valorAtualizado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_DescricaoInvalida_Quando_AtualizarServicoCatalogo_Entao_DeveLancarDomainException(string descricao)
    {
        // Arrange
        var servico = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        // Act
        var acao = () => servico.Atualizar(descricao, ServicoCatalogoTestDataFactory.ValorPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(ServicoCatalogoErrorMessages.DescricaoObrigatoria);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_AtualizarServicoCatalogo_Entao_DeveLancarDomainException(decimal valor)
    {
        // Arrange
        var servico = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        // Act
        var acao = () => servico.Atualizar(ServicoCatalogoTestDataFactory.DescricaoPadrao, valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(ServicoCatalogoErrorMessages.ValorMaiorQueZero);
    }
}

using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;

public class CadastrarPecaInsumoCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_DadosValidos_Quando_CadastrarPecaInsumoCatalogo_Entao_DeveCadastrarPecaInsumoCatalogo()
    {
        // Arrange
        var repository = CriarRepository();

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarCadastrarPecaInsumoCatalogoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.Descricao.Should().Be(PecaInsumoCatalogoTestDataFactory.DescricaoPadrao);
        resultado.Valor.Tipo.Should().Be(PecaInsumoCatalogoTestDataFactory.TipoPadrao);
        resultado.Valor.Valor.Should().Be(PecaInsumoCatalogoTestDataFactory.ValorPadrao);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.Is<PecaInsumoCatalogo>(item =>
                    item.Descricao == PecaInsumoCatalogoTestDataFactory.DescricaoPadrao
                    && item.Tipo == PecaInsumoCatalogoTestDataFactory.TipoPadrao
                    && item.Valor == PecaInsumoCatalogoTestDataFactory.ValorPadrao),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_DescricaoInvalida_Quando_CadastrarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao(
        string descricao)
    {
        // Arrange
        var repository = CriarRepository();

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarCadastrarPecaInsumoCatalogoRequestValido(
            descricao: descricao);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.IsAny<PecaInsumoCatalogo>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_TipoInvalido_Quando_CadastrarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = CriarRepository();

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarCadastrarPecaInsumoCatalogoRequestValido(
            tipo: (TipoPecaInsumo)99);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.IsAny<PecaInsumoCatalogo>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_ValorInvalido_Quando_CadastrarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = CriarRepository();

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarCadastrarPecaInsumoCatalogoRequestValido(
            valor: 0m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.IsAny<PecaInsumoCatalogo>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IPecaInsumoCatalogoRepository> CriarRepository()
    {
        return new Mock<IPecaInsumoCatalogoRepository>();
    }

    private static CadastrarPecaInsumoCatalogoUseCase CriarUseCase(
        Mock<IPecaInsumoCatalogoRepository> repository)
    {
        return new CadastrarPecaInsumoCatalogoUseCase(
            repository.Object,
            new CadastrarPecaInsumoCatalogoValidator(),
            MapperFactory.Criar());
    }
}
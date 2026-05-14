using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;
using OficinaMecanica.Application.Common;
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
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = new CadastrarPecaInsumoCatalogoRequest(
            "Filtro de óleo",
            TipoPecaInsumo.PECA,
            45m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.Descricao.Should().Be(request.Descricao);
        resultado.Valor.Tipo.Should().Be(request.Tipo);
        resultado.Valor.Valor.Should().Be(request.Valor);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.Is<PecaInsumoCatalogo>(item =>
                    item.Descricao == request.Descricao
                    && item.Tipo == request.Tipo
                    && item.Valor == request.Valor),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_DescricaoVazia_Quando_CadastrarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = new CadastrarPecaInsumoCatalogoRequest(
            string.Empty,
            TipoPecaInsumo.PECA,
            45m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.AdicionarAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_TipoInvalido_Quando_CadastrarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = new CadastrarPecaInsumoCatalogoRequest(
            "Filtro de óleo",
            (TipoPecaInsumo)99,
            45m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.AdicionarAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_ValorInvalido_Quando_CadastrarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = new CadastrarPecaInsumoCatalogoRequest(
            "Filtro de óleo",
            TipoPecaInsumo.PECA,
            0m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.AdicionarAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()),
            Times.Never);
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
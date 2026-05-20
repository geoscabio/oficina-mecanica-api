using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.AtualizarPecaInsumoCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.UnitTests.Administrativo.PecaInsumoCatalogoUseCases.AtualizarPecaInsumoCatalogo;

public class AtualizarPecaInsumoCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveAtualizarPecaInsumoCatalogo()
    {
        // Arrange
        var item = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();

        var repository = CriarRepository(item);

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarAtualizarPecaInsumoCatalogoRequestValido(item.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(item.Id);
        resultado.Valor.Descricao.Should().Be(request.Descricao);
        resultado.Valor.Tipo.Should().Be(request.Tipo);
        resultado.Valor.Valor.Should().Be(request.Valor);

        repository.Verify(repo => repo.ObterPorIdAsync(request.PecaInsumoCatalogoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<PecaInsumoCatalogo>(itemAtualizado =>
                    itemAtualizado.Id == item.Id
                    && itemAtualizado.Descricao == request.Descricao
                    && itemAtualizado.Tipo == request.Tipo
                    && itemAtualizado.Valor == request.Valor),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_PecaInsumoCatalogoInexistente_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarAtualizarPecaInsumoCatalogoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(repo => repo.ObterPorIdAsync(request.PecaInsumoCatalogoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarAtualizarPecaInsumoCatalogoRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_DescricaoInvalida_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao(string descricao)
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarAtualizarPecaInsumoCatalogoRequestValido(descricao: descricao);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_TipoInvalido_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarAtualizarPecaInsumoCatalogoRequestValido(tipo: (TipoPecaInsumo)99);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_ValorInvalido_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarAtualizarPecaInsumoCatalogoRequestValido(valor: 0m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IPecaInsumoCatalogoRepository> CriarRepository(PecaInsumoCatalogo? item)
    {
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        return repository;
    }

    private static AtualizarPecaInsumoCatalogoUseCase CriarUseCase(Mock<IPecaInsumoCatalogoRepository> repository)
    {
        return new AtualizarPecaInsumoCatalogoUseCase(repository.Object, new AtualizarPecaInsumoCatalogoValidator(), MapperFactory.Criar());
    }
}
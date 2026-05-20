using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.AtualizarEstoque;
using OficinaMecanica.Application.GestaoEstoque.ValidationMessages;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoEstoque.Factories;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;

namespace OficinaMecanica.Application.UnitTests.GestaoEstoque.EstoqueUseCases.AtualizarEstoque;

public class AtualizarEstoqueUseCaseTests
{
    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarEstoque_Entao_DeveAtualizarQuantidadeDisponivel()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(pecaInsumoCatalogoId);

        var repository = CriarRepository(estoque);

        var useCase = CriarUseCase(repository);

        var request = EstoqueTestDataFactory.CriarAtualizarEstoqueRequestValido(pecaInsumoCatalogoId);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.PecaInsumoCatalogoId.Should().Be(pecaInsumoCatalogoId);
        resultado.Valor.QuantidadeDisponivel.Should().Be(request.QuantidadeDisponivel);
        estoque.ObterItem(pecaInsumoCatalogoId).QuantidadeDisponivel.Should().Be(request.QuantidadeDisponivel);

        repository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.AtualizarAsync(estoque, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_ItemInexistente_Quando_AtualizarEstoque_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(EstoqueTestDataFactory.CriarEstoquePadrao());

        var useCase = CriarUseCase(repository);

        var request = EstoqueTestDataFactory.CriarAtualizarEstoqueRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(EstoqueValidationMessages.ItemEstoqueNaoEncontrado);
        resultado.Erro!.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<Estoque>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_EstoqueInexistente_Quando_AtualizarEstoque_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = EstoqueTestDataFactory.CriarAtualizarEstoqueRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(EstoqueValidationMessages.ItemEstoqueNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<Estoque>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_PecaInsumoCatalogoIdVazio_Quando_AtualizarEstoque_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(repository);

        var request = EstoqueTestDataFactory.CriarAtualizarEstoqueRequestValido(
            Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(EstoqueValidationMessages.PecaInsumoCatalogoObrigatorio);
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<Estoque>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_QuantidadeDisponivelNegativa_Quando_AtualizarEstoque_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(repository);

        var request = EstoqueTestDataFactory.CriarAtualizarEstoqueRequestValido(
            quantidadeDisponivel: -1);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(EstoqueValidationMessages.QuantidadeDisponivelNaoNegativa);
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<Estoque>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IEstoqueRepository> CriarRepository(Estoque? estoque)
    {
        var repository = new Mock<IEstoqueRepository>();

        repository
            .Setup(repo => repo.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(estoque);

        return repository;
    }

    private static AtualizarEstoqueUseCase CriarUseCase(Mock<IEstoqueRepository> repository)
    {
        return new AtualizarEstoqueUseCase(
            repository.Object,
            new AtualizarEstoqueValidator(),
            MapperFactory.Criar());
    }
}

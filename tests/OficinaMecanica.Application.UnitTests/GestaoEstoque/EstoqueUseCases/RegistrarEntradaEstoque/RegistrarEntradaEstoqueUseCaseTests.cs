using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.RegistrarEntradaEstoque;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoEstoque.Factories;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;

namespace OficinaMecanica.Application.UnitTests.GestaoEstoque.EstoqueUseCases.RegistrarEntradaEstoque;

public class RegistrarEntradaEstoqueUseCaseTests
{
    [Fact]
    public async Task Dado_ItemExistente_Quando_RegistrarEntradaEstoque_Entao_DeveSomarQuantidade()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(pecaInsumoCatalogoId, quantidadeDisponivel: 10);

        var repository = CriarRepository(estoque);

        var useCase = CriarUseCase(repository);

        var request =
            EstoqueTestDataFactory.CriarRegistrarEntradaEstoqueRequestValido(pecaInsumoCatalogoId, quantidade: 5);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        resultado.Valor.Should().NotBeNull();

        resultado.Valor!.QuantidadeDisponivel.Should().Be(15);

        repository.Verify(repo => repo.AtualizarAsync(estoque, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_ItemInexistenteParaPecaInsumo_Quando_RegistrarEntradaEstoque_Entao_DeveCriarNovoItem()
    {
        // Arrange
        var pecaInsumoJaExistenteId = Guid.NewGuid();

        var pecaInsumoNovaId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(pecaInsumoJaExistenteId);

        var repository = CriarRepository(estoque);

        var useCase = CriarUseCase(repository);

        var request =
            EstoqueTestDataFactory.CriarRegistrarEntradaEstoqueRequestValido(pecaInsumoNovaId);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        resultado.Valor.Should().NotBeNull();

        resultado.Valor!.PecaInsumoCatalogoId.Should().Be(pecaInsumoNovaId);

        resultado.Valor.QuantidadeDisponivel.Should().Be(request.Quantidade);

        estoque.ItensEstoque.Should().Contain(item => item.PecaInsumoCatalogoId == pecaInsumoNovaId);

        repository.Verify(repo => repo.AtualizarAsync(estoque, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_EstoqueInexistente_Quando_RegistrarEntradaEstoque_Entao_DeveCriarEstoqueComItem()
    {
        // Arrange
        var repository = new Mock<IEstoqueRepository>();

        repository
            .Setup(repo => repo.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Estoque?)null);

        var useCase = CriarUseCase(repository);

        var request =
            EstoqueTestDataFactory.CriarRegistrarEntradaEstoqueRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        resultado.Valor.Should().NotBeNull();

        resultado.Valor!.PecaInsumoCatalogoId.Should().Be(request.PecaInsumoCatalogoId);

        resultado.Valor.QuantidadeDisponivel.Should().Be(request.Quantidade);

        repository.Verify(repo => repo.AtualizarAsync(It.Is<Estoque>(estoque => estoque.ItensEstoque.Any(item => item.PecaInsumoCatalogoId == request.PecaInsumoCatalogoId)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_PecaInsumoCatalogoIdVazio_Quando_RegistrarEntradaEstoque_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(repository);

        var request =
            EstoqueTestDataFactory.CriarRegistrarEntradaEstoqueRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterAsync(It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_QuantidadeInvalida_Quando_RegistrarEntradaEstoque_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(repository);

        var request =
            EstoqueTestDataFactory.CriarRegistrarEntradaEstoqueRequestValido(quantidade: 0);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterAsync(It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IEstoqueRepository> CriarRepository(Estoque estoque)
    {
        var repository = new Mock<IEstoqueRepository>();

        repository
            .Setup(repo => repo.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(estoque);

        return repository;
    }

    private static RegistrarEntradaEstoqueUseCase CriarUseCase(Mock<IEstoqueRepository> repository)
    {
        return new RegistrarEntradaEstoqueUseCase(repository.Object, new RegistrarEntradaEstoqueValidator(), MapperFactory.Criar());
    }
}

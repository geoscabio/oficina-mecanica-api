using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ConsultarItemEstoque;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoEstoque.Factories;
using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;

namespace OficinaMecanica.Application.UnitTests.GestaoEstoque.EstoqueUseCases.ConsultarItemEstoque;

public class ConsultarItemEstoqueUseCaseTests
{
    [Fact]
    public async Task Dado_ItemExistente_Quando_ConsultarItemEstoque_Entao_DeveRetornarItem()
    {
        // Arrange
        var estoque = EstoqueTestDataFactory.CriarEstoqueComItens(1);

        var itemEstoque = estoque.ItensEstoque.First();

        var repository = CriarRepository(itemEstoque);

        var useCase = CriarUseCase(repository);

        var request = EstoqueTestDataFactory.CriarConsultarItemEstoqueRequestValido(itemEstoque.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        resultado.Valor.Should().NotBeNull();

        resultado.Valor!.Id.Should().Be(itemEstoque.Id);
    }

    [Fact]
    public async Task Dado_ItemInexistente_Quando_ConsultarItemEstoque_Entao_DeveRetornarNaoEncontrado()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request =
            EstoqueTestDataFactory.CriarConsultarItemEstoqueRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Tipo.Should().Be(TipoErro.NaoEncontrado);
    }

    [Fact]
    public async Task Dado_ItemEstoqueIdVazio_Quando_ConsultarItemEstoque_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(repository);

        var request =
            EstoqueTestDataFactory.CriarConsultarItemEstoqueRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterItemPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IEstoqueRepository> CriarRepository(ItemEstoque? itemEstoque)
    {
        var repository = new Mock<IEstoqueRepository>();

        repository
            .Setup(repo => repo.ObterItemPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(itemEstoque);

        return repository;
    }

    private static ConsultarItemEstoqueUseCase CriarUseCase(Mock<IEstoqueRepository> repository)
    {
        return new ConsultarItemEstoqueUseCase(repository.Object, new ConsultarItemEstoqueValidator(), MapperFactory.Criar());
    }
}
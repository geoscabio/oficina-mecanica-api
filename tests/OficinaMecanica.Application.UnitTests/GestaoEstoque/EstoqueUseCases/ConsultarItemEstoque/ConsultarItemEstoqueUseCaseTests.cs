using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ConsultarItemEstoque;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoEstoque.Builders;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
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

        var repository = CriarRepository(estoque);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(
            new ConsultarItemEstoqueRequest(itemEstoque.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(itemEstoque.Id);
    }

    [Fact]
    public async Task Dado_ItemInexistente_Quando_ConsultarItemEstoque_Entao_DeveRetornarNaoEncontrado()
    {
        // Arrange
        var estoque = EstoqueTestDataFactory.CriarEstoqueComItens(1);

        var repository = CriarRepository(estoque);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(
            new ConsultarItemEstoqueRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.NaoEncontrado);
    }

    [Fact]
    public async Task Dado_IdInvalido_Quando_ConsultarItemEstoque_Entao_DeveRetornarErroValidacao()
    {
        // Arrange
        var repository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(
            new ConsultarItemEstoqueRequest(Guid.Empty));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repository => repository.ObterAsync(It.IsAny<CancellationToken>()),
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

    private static ConsultarItemEstoqueUseCase CriarUseCase(
        Mock<IEstoqueRepository> repository)
    {
        return new ConsultarItemEstoqueUseCase(
            repository.Object,
            new ConsultarItemEstoqueValidator(),
            MapperFactory.Criar());
    }
}
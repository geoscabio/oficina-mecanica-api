using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ConsultarPecaInsumoCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.UnitTests.Administrativo.PecaInsumoCatalogoUseCases.ConsultarPecaInsumoCatalogo;

public class ConsultarPecaInsumoCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_PecaInsumoCatalogoExistente_Quando_ConsultarPecaInsumoCatalogo_Entao_DeveRetornarPecaInsumoCatalogo()
    {
        // Arrange
        var item = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(item.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarPecaInsumoCatalogoRequest(item.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(item.Id);
        resultado.Valor.Descricao.Should().Be(item.Descricao);
        resultado.Valor.Tipo.Should().Be(item.Tipo);
        resultado.Valor.Valor.Should().Be(item.Valor);
    }

    [Fact]
    public async Task Dado_PecaInsumoCatalogoInexistente_Quando_ConsultarPecaInsumoCatalogo_Entao_DeveRetornarFalha()
    {
        //  Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        //  Act
        var resultado = await useCase.ExecuteAsync(new ConsultarPecaInsumoCatalogoRequest(Guid.NewGuid()));

        //  Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        //  Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        //  Act
        var resultado = await useCase.ExecuteAsync(new ConsultarPecaInsumoCatalogoRequest(Guid.Empty));

        //  Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static ConsultarPecaInsumoCatalogoUseCase CriarUseCase(
        Mock<IPecaInsumoCatalogoRepository> repository)
    {
        return new ConsultarPecaInsumoCatalogoUseCase(
            repository.Object,
            new ConsultarPecaInsumoCatalogoValidator(),
            MapperFactory.Criar());
    }
}
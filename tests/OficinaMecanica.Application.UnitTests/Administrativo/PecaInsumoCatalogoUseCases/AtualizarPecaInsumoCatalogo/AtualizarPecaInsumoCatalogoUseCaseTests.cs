using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.AtualizarPecaInsumoCatalogo;
using OficinaMecanica.Application.Common;
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
        var item = PecaInsumoCatalogo.Criar("Filtro de óleo", TipoPecaInsumo.PECA, 45m);
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(item.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(item.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(item.Id);
        resultado.Valor.Descricao.Should().Be(request.Descricao);
        resultado.Valor.Tipo.Should().Be(request.Tipo);
        resultado.Valor.Valor.Should().Be(request.Valor);

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
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(Guid.NewGuid());

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.AtualizarAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_DescricaoInvalida_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao(string descricao)
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(Guid.NewGuid()) with { Descricao = descricao };

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static AtualizarPecaInsumoCatalogoUseCase CriarUseCase(
        Mock<IPecaInsumoCatalogoRepository> repository)
    {
        return new AtualizarPecaInsumoCatalogoUseCase(
            repository.Object,
            new AtualizarPecaInsumoCatalogoValidator(),
            MapperFactory.Criar());
    }

    private static AtualizarPecaInsumoCatalogoRequest CriarRequestValido(Guid itemId)
    {
        return new AtualizarPecaInsumoCatalogoRequest(
            PecaInsumoCatalogoId: itemId,
            Descricao: "Óleo 5W30",
            Tipo: TipoPecaInsumo.INSUMO,
            Valor: 38m);
    }
}
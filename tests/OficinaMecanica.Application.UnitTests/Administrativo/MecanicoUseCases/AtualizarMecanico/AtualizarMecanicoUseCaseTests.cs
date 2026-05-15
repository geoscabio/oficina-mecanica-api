using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.AtualizarMecanico;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.UnitTests.Administrativo.MecanicoUseCases.AtualizarMecanico;

public class AtualizarMecanicoUseCaseTests
{
    private const string NomeAtualizado = "Carlos Silva";
    private const string FuncionalAtualizado = "MEC-002";

    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarMecanico_Entao_DeveAtualizarMecanico()
    {
        // Arrange
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();
        var repository = new Mock<IMecanicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(mecanico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mecanico);

        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(mecanico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(mecanico.Id);
        resultado.Valor.Nome.Should().Be(request.Nome);
        resultado.Valor.Funcional.Should().Be(request.Funcional);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<Mecanico>(mecanicoAtualizado =>
                    mecanicoAtualizado.Id == mecanico.Id
                    && mecanicoAtualizado.Nome == request.Nome
                    && mecanicoAtualizado.Funcional == request.Funcional),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_MecanicoInexistente_Quando_AtualizarMecanico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(Guid.NewGuid());

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(MecanicoErrorMessages.MecanicoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.AtualizarAsync(It.IsAny<Mecanico>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_AtualizarMecanico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();
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
    public async Task Dado_NomeInvalido_Quando_AtualizarMecanico_Entao_DeveRetornarFalhaDeValidacao(string nome)
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(Guid.NewGuid()) with { Nome = nome };

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.AtualizarAsync(It.IsAny<Mecanico>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static AtualizarMecanicoUseCase CriarUseCase(Mock<IMecanicoRepository> repository)
    {
        return new AtualizarMecanicoUseCase(
            repository.Object,
            new AtualizarMecanicoValidator(),
            MapperFactory.Criar());
    }

    private static AtualizarMecanicoRequest CriarRequestValido(Guid mecanicoId)
    {
        return new AtualizarMecanicoRequest(
            mecanicoId,
            NomeAtualizado,
            FuncionalAtualizado);
    }
}
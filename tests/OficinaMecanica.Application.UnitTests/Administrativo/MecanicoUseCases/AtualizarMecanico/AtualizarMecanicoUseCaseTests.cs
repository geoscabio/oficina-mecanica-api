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
    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarMecanico_Entao_DeveAtualizarMecanico()
    {
        // Arrange
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();

        var repository = CriarRepository(mecanico);

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarAtualizarMecanicoRequestValido(mecanico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(mecanico.Id);
        resultado.Valor.Nome.Should().Be(request.Nome);
        resultado.Valor.Funcional.Should().Be(request.Funcional);

        repository.Verify(repo => repo.ObterPorIdAsync(request.MecanicoId, It.IsAny<CancellationToken>()), Times.Once);

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
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarAtualizarMecanicoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(MecanicoErrorMessages.MecanicoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(repo => repo.ObterPorIdAsync(request.MecanicoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<Mecanico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_AtualizarMecanico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarAtualizarMecanicoRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<Mecanico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_NomeInvalido_Quando_AtualizarMecanico_Entao_DeveRetornarFalhaDeValidacao(string nome)
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarAtualizarMecanicoRequestValido(nome: nome);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<Mecanico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_FuncionalInvalida_Quando_AtualizarMecanico_Entao_DeveRetornarFalhaDeValidacao(string funcional)
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarAtualizarMecanicoRequestValido(funcional: funcional);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<Mecanico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IMecanicoRepository> CriarRepository(Mecanico? mecanico)
    {
        var repository = new Mock<IMecanicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mecanico);

        return repository;
    }

    private static AtualizarMecanicoUseCase CriarUseCase(Mock<IMecanicoRepository> repository)
    {
        return new AtualizarMecanicoUseCase(repository.Object, new AtualizarMecanicoValidator(), MapperFactory.Criar());
    }
}
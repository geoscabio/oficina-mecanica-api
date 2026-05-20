using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.AtualizarVeiculo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.AtualizarVeiculo;

public class AtualizarVeiculoUseCaseTests
{
    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarVeiculo_Entao_DeveAtualizarVeiculo()
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();

        var repository = CriarRepository(veiculo);

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarAtualizarVeiculoRequestValido(veiculo.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(veiculo.Id);
        resultado.Valor.Placa.Should().Be(VeiculoTestDataFactory.PlacaAtualizadaNormalizada);
        resultado.Valor.Marca.Should().Be(request.Marca);
        resultado.Valor.Modelo.Should().Be(request.Modelo);
        resultado.Valor.Ano.Should().Be(request.Ano);

        repository.Verify(repo => repo.ObterPorIdAsync(request.VeiculoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<Veiculo>(veiculoAtualizado =>
                    veiculoAtualizado.Id == veiculo.Id
                    && veiculoAtualizado.Placa.NumeroPlaca == VeiculoTestDataFactory.PlacaAtualizadaNormalizada
                    && veiculoAtualizado.Marca == request.Marca
                    && veiculoAtualizado.Modelo == request.Modelo
                    && veiculoAtualizado.Ano == request.Ano),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_VeiculoInexistente_Quando_AtualizarVeiculo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarAtualizarVeiculoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(VeiculoErrorMessages.VeiculoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(repo => repo.ObterPorIdAsync(request.VeiculoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_AtualizarVeiculo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IVeiculoRepository>();

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarAtualizarVeiculoRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_MarcaInvalida_Quando_AtualizarVeiculo_Entao_DeveRetornarFalhaDeValidacao(string marca)
    {
        // Arrange
        var repository = new Mock<IVeiculoRepository>();

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarAtualizarVeiculoRequestValido(marca: marca);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IVeiculoRepository> CriarRepository(Veiculo? veiculo)
    {
        var repository = new Mock<IVeiculoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        return repository;
    }

    private static AtualizarVeiculoUseCase CriarUseCase(Mock<IVeiculoRepository> repository)
    {
        return new AtualizarVeiculoUseCase(repository.Object, new AtualizarVeiculoValidator(), MapperFactory.Criar());
    }
}
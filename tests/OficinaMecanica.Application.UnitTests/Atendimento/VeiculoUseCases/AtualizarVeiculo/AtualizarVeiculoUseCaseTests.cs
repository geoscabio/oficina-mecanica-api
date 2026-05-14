using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.AtualizarVeiculo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Builders;
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
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();
        var repository = new Mock<IVeiculoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(veiculo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(veiculo.Id);

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(veiculo.Id);
        resultado.Valor.Placa.Should().Be("XYZ9876");
        resultado.Valor.Marca.Should().Be(request.Marca);
        resultado.Valor.Modelo.Should().Be(request.Modelo);
        resultado.Valor.Ano.Should().Be(request.Ano);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<Veiculo>(veiculoAtualizado =>
                    veiculoAtualizado.Id == veiculo.Id
                    && veiculoAtualizado.Placa.NumeroPlaca == "XYZ9876"
                    && veiculoAtualizado.Marca == request.Marca
                    && veiculoAtualizado.Modelo == request.Modelo
                    && veiculoAtualizado.Ano == request.Ano),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_VeiculoInexistente_Quando_AtualizarVeiculo_Entao_DeveRetornarFalha()
    {
        var repository = new Mock<IVeiculoRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(Guid.NewGuid());

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(VeiculoErrorMessages.VeiculoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.AtualizarAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_AtualizarVeiculo_Entao_DeveRetornarFalhaDeValidacao()
    {
        var repository = new Mock<IVeiculoRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(Guid.Empty);

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_MarcaInvalida_Quando_AtualizarVeiculo_Entao_DeveRetornarFalhaDeValidacao(string marca)
    {
        var repository = new Mock<IVeiculoRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(Guid.NewGuid()) with { Marca = marca };

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static AtualizarVeiculoUseCase CriarUseCase(Mock<IVeiculoRepository> repository)
    {
        return new AtualizarVeiculoUseCase(
            repository.Object,
            new AtualizarVeiculoValidator(),
            MapperFactory.Criar());
    }

    private static AtualizarVeiculoRequest CriarRequestValido(Guid veiculoId)
    {
        return new AtualizarVeiculoRequest(
            VeiculoId: veiculoId,
            Placa: "XYZ-9876",
            Marca: "Honda",
            Modelo: "Civic",
            Ano: 2022);
    }
}
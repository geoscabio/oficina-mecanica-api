using FluentAssertions;
using FluentValidation;
using Moq;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Builders;
using OficinaMecanica.Application.UnitTests.Atendimento.Builders;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public class AbrirOrdemServicoUseCaseTests
{
    [Fact]
    public async Task Dado_RequestValido_Quando_AbrirOrdemServico_Entao_DevePersistirOrdemServicoERetornarSucesso()
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var mecanicoRepository = new Mock<IMecanicoRepository>();

        veiculoRepository
            .Setup(repo => repo.ObterPorIdAsync(veiculo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        mecanicoRepository
            .Setup(repo => repo.ObterPorIdAsync(mecanico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mecanico);

        var useCase = CriarUseCase(ordemServicoRepository, veiculoRepository, mecanicoRepository);
        var request = new AbrirOrdemServicoRequest(veiculo.Id, mecanico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.Numero.Should().BeGreaterThan(0);
        resultado.Valor.Status.Should().Be("RECEBIDA");
        resultado.Valor.ValorTotal.Should().Be(0);
        resultado.Valor.DataInicio.Should().NotBeNull();
        resultado.Valor.DataFim.Should().BeNull();
        resultado.Valor.VeiculoId.Should().Be(veiculo.Id);
        resultado.Valor.MecanicoId.Should().Be(mecanico.Id);

        ordemServicoRepository.Verify(repo => repo.AdicionarAsync(It.Is<OrdemServico>(ordemServico =>
            ordemServico.VeiculoId == veiculo.Id
            && ordemServico.MecanicoId == mecanico.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_VeiculoInexistente_Quando_AbrirOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var mecanicoRepository = new Mock<IMecanicoRepository>();

        mecanicoRepository
            .Setup(repo => repo.ObterPorIdAsync(mecanico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mecanico);

        var useCase = CriarUseCase(ordemServicoRepository, veiculoRepository, mecanicoRepository);
        var request = new AbrirOrdemServicoRequest(Guid.NewGuid(), mecanico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be("Veiculo nao encontrado.");

        ordemServicoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_MecanicoInexistente_Quando_AbrirOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var mecanicoRepository = new Mock<IMecanicoRepository>();

        veiculoRepository
            .Setup(repo => repo.ObterPorIdAsync(veiculo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        var useCase = CriarUseCase(ordemServicoRepository, veiculoRepository, mecanicoRepository);
        var request = new AbrirOrdemServicoRequest(veiculo.Id, Guid.NewGuid());

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be("Mecanico nao encontrado.");

        ordemServicoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_VeiculoIdVazio_Quando_AbrirOrdemServico_Entao_DeveLancarValidationException()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var mecanicoRepository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, veiculoRepository, mecanicoRepository);
        var request = new AbrirOrdemServicoRequest(Guid.Empty, Guid.NewGuid());

        // Act
        var acao = () => useCase.ExecuteAsync(request);

        // Assert
        await acao.Should().ThrowAsync<ValidationException>();
    }

    private static AbrirOrdemServicoUseCase CriarUseCase(
        Mock<IOrdemServicoRepository> ordemServicoRepository,
        Mock<IVeiculoRepository> veiculoRepository,
        Mock<IMecanicoRepository> mecanicoRepository)
    {
        return new AbrirOrdemServicoUseCase(
            ordemServicoRepository.Object,
            veiculoRepository.Object,
            mecanicoRepository.Object,
            new AbrirOrdemServicoValidator(),
            MapperFactory.Criar());
    }
}



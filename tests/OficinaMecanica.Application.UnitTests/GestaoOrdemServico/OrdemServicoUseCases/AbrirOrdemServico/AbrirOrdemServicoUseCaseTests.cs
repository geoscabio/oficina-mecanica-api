using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public class AbrirOrdemServicoUseCaseTests
{
    private const int ProximoNumeroOrdemServico = 123;

    [Fact]
    public async Task Dado_RequestValido_Quando_AbrirOrdemServico_Entao_DevePersistirOrdemServicoERetornarSucesso()
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();

        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        ordemServicoRepository
            .Setup(repo => repo.ObterProximoNumeroAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ProximoNumeroOrdemServico);

        var veiculoRepository = CriarVeiculoRepository(veiculo);

        var mecanicoRepository = CriarMecanicoRepository(mecanico);

        var useCase = CriarUseCase(
            ordemServicoRepository,
            veiculoRepository,
            mecanicoRepository);

        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(
            veiculo.Id,
            mecanico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.Numero.Should().Be(ProximoNumeroOrdemServico);
        resultado.Valor.Status.Should().Be(OrdemServicoTestDataFactory.StatusRecebida);
        resultado.Valor.ValorTotal.Should().Be(0);
        resultado.Valor.DataInicio.Should().NotBeNull();
        resultado.Valor.DataFim.Should().BeNull();
        resultado.Valor.VeiculoId.Should().Be(veiculo.Id);
        resultado.Valor.MecanicoId.Should().Be(mecanico.Id);

        veiculoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.VeiculoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        mecanicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.MecanicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        ordemServicoRepository.Verify(
            repo => repo.ObterProximoNumeroAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        ordemServicoRepository.Verify(
            repo => repo.AdicionarAsync(
                It.Is<OrdemServico>(ordemServico =>
                    ordemServico.VeiculoId == veiculo.Id
                    && ordemServico.MecanicoId == mecanico.Id
                    && ordemServico.Numero == ProximoNumeroOrdemServico),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_VeiculoInexistente_Quando_AbrirOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var veiculoRepository = CriarVeiculoRepository(null);

        var mecanicoRepository = CriarMecanicoRepository(mecanico);

        var useCase = CriarUseCase(
            ordemServicoRepository,
            veiculoRepository,
            mecanicoRepository);

        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(
            mecanicoId: mecanico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(VeiculoErrorMessages.VeiculoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        veiculoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.VeiculoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        mecanicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.ObterProximoNumeroAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.AdicionarAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_MecanicoInexistente_Quando_AbrirOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var veiculoRepository = CriarVeiculoRepository(veiculo);

        var mecanicoRepository = CriarMecanicoRepository(null);

        var useCase = CriarUseCase(
            ordemServicoRepository,
            veiculoRepository,
            mecanicoRepository);

        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(
            veiculo.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(MecanicoErrorMessages.MecanicoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        veiculoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.VeiculoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        mecanicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.MecanicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        ordemServicoRepository.Verify(
            repo => repo.ObterProximoNumeroAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.AdicionarAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_VeiculoIdVazio_Quando_AbrirOrdemServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var veiculoRepository = new Mock<IVeiculoRepository>();

        var mecanicoRepository = new Mock<IMecanicoRepository>();

        var useCase = CriarUseCase(
            ordemServicoRepository,
            veiculoRepository,
            mecanicoRepository);

        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(
            veiculoId: Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        veiculoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        mecanicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.ObterProximoNumeroAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.AdicionarAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IVeiculoRepository> CriarVeiculoRepository(Veiculo? veiculo)
    {
        var repository = new Mock<IVeiculoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        return repository;
    }

    private static Mock<IMecanicoRepository> CriarMecanicoRepository(Mecanico? mecanico)
    {
        var repository = new Mock<IMecanicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mecanico);

        return repository;
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

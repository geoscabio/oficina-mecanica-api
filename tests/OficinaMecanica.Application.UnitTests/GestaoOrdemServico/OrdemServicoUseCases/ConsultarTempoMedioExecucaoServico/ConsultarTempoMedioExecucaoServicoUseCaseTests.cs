using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarTempoMedioExecucaoServico;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.ConsultarTempoMedioExecucaoServico;

public class ConsultarTempoMedioExecucaoServicoUseCaseTests
{
    [Fact]
    public async Task Dado_ServicoCatalogoExistente_Quando_ConsultarTempoMedioExecucaoServico_Entao_DeveRetornarTempoMedio()
    {
        // Arrange
        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        var servicoCatalogoRepository = CriarServicoCatalogoRepository(servicoCatalogo);

        var ordemServicoRepository = CriarOrdemServicoRepository(ServicoCatalogoTestDataFactory.TempoMedioExecucaoPadrao);

        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        var request = ServicoCatalogoTestDataFactory.CriarConsultarTempoMedioExecucaoServicoRequestValido(servicoCatalogo.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.ServicoCatalogoId.Should().Be(servicoCatalogo.Id);
        resultado.Valor.Descricao.Should().Be(ServicoCatalogoTestDataFactory.DescricaoPadrao);
        resultado.Valor.Valor.Should().Be(ServicoCatalogoTestDataFactory.ValorPadrao);
        resultado.Valor.TempoMedioExecucaoEmMinutos.Should().Be(ServicoCatalogoTestDataFactory.TempoMedioExecucaoPadrao);

        servicoCatalogoRepository.Verify(repo => repo.ObterPorIdAsync(request.ServicoCatalogoId, It.IsAny<CancellationToken>()), Times.Once);

        ordemServicoRepository.Verify(repo => repo.ObterTempoMedioExecucaoServicoAsync(servicoCatalogo.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_ServicoCatalogoSemOrdensFinalizadas_Quando_ConsultarTempoMedioExecucaoServico_Entao_DeveRetornarTempoMedioNulo()
    {
        // Arrange
        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        var servicoCatalogoRepository = CriarServicoCatalogoRepository(servicoCatalogo);

        var ordemServicoRepository = CriarOrdemServicoRepository(null);

        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        var request = ServicoCatalogoTestDataFactory.CriarConsultarTempoMedioExecucaoServicoRequestValido(servicoCatalogo.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.ServicoCatalogoId.Should().Be(servicoCatalogo.Id);
        resultado.Valor.TempoMedioExecucaoEmMinutos.Should().BeNull();

        servicoCatalogoRepository.Verify(repo => repo.ObterPorIdAsync(request.ServicoCatalogoId, It.IsAny<CancellationToken>()), Times.Once);

        ordemServicoRepository.Verify(repo => repo.ObterTempoMedioExecucaoServicoAsync(servicoCatalogo.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_ServicoCatalogoInexistente_Quando_ConsultarTempoMedioExecucaoServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var servicoCatalogoRepository = CriarServicoCatalogoRepository(null);

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        var request = ServicoCatalogoTestDataFactory.CriarConsultarTempoMedioExecucaoServicoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        servicoCatalogoRepository.Verify(repo => repo.ObterPorIdAsync(request.ServicoCatalogoId, It.IsAny<CancellationToken>()), Times.Once);

        ordemServicoRepository.Verify(repo => repo.ObterTempoMedioExecucaoServicoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarTempoMedioExecucaoServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        var request = ServicoCatalogoTestDataFactory.CriarConsultarTempoMedioExecucaoServicoRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        servicoCatalogoRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        ordemServicoRepository.Verify(repo => repo.ObterTempoMedioExecucaoServicoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IServicoCatalogoRepository> CriarServicoCatalogoRepository(ServicoCatalogo? servicoCatalogo)
    {
        var repository = new Mock<IServicoCatalogoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicoCatalogo);

        return repository;
    }

    private static Mock<IOrdemServicoRepository> CriarOrdemServicoRepository(double? tempoMedio)
    {
        var repository = new Mock<IOrdemServicoRepository>();

        repository
            .Setup(repo => repo.ObterTempoMedioExecucaoServicoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tempoMedio);

        return repository;
    }

    private static ConsultarTempoMedioExecucaoServicoUseCase CriarUseCase(Mock<IServicoCatalogoRepository> servicoCatalogoRepository, Mock<IOrdemServicoRepository> ordemServicoRepository)
    {
        return new ConsultarTempoMedioExecucaoServicoUseCase(servicoCatalogoRepository.Object, ordemServicoRepository.Object, new ConsultarTempoMedioExecucaoServicoValidator(), MapperFactory.Criar());
    }
}

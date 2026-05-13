using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarTempoMedioExecucaoServico;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Builders;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Administrativo.ServicoCatalogoUseCases.ConsultarTempoMedioExecucaoServico;

public class ConsultarTempoMedioExecucaoServicoUseCaseTests
{
    [Fact]
    public async Task Dado_ServicoCatalogoComHistorico_Quando_ConsultarTempoMedioExecucaoServico_Entao_DeveRetornarTempoMedio()
    {
        // Arrange
        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();
        servicoCatalogoRepository
            .Setup(repo => repo.ObterPorIdAsync(servicoCatalogo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicoCatalogo);

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        ordemServicoRepository
            .Setup(repo => repo.ObterTempoMedioExecucaoServicoAsync(servicoCatalogo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(45d);

        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarTempoMedioExecucaoServicoRequest(servicoCatalogo.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.ServicoCatalogoId.Should().Be(servicoCatalogo.Id);
        resultado.Valor.Descricao.Should().Be(servicoCatalogo.Descricao);
        resultado.Valor.Valor.Should().Be(servicoCatalogo.Valor);
        resultado.Valor.TempoMedioExecucaoEmMinutos.Should().Be(45d);
    }

    [Fact]
    public async Task Dado_ServicoCatalogoSemHistorico_Quando_ConsultarTempoMedioExecucaoServico_Entao_DeveRetornarTempoMedioNulo()
    {
        // Arrange
        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();
        servicoCatalogoRepository
            .Setup(repo => repo.ObterPorIdAsync(servicoCatalogo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicoCatalogo);

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        ordemServicoRepository
            .Setup(repo => repo.ObterTempoMedioExecucaoServicoAsync(servicoCatalogo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((double?)null);

        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarTempoMedioExecucaoServicoRequest(servicoCatalogo.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.TempoMedioExecucaoEmMinutos.Should().BeNull();
    }

    [Fact]
    public async Task Dado_ServicoCatalogoInexistente_Quando_ConsultarTempoMedioExecucaoServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarTempoMedioExecucaoServicoRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarTempoMedioExecucaoServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarTempoMedioExecucaoServicoRequest(Guid.Empty));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static ConsultarTempoMedioExecucaoServicoUseCase CriarUseCase(
        Mock<IServicoCatalogoRepository> servicoCatalogoRepository,
        Mock<IOrdemServicoRepository> ordemServicoRepository)
    {
        return new ConsultarTempoMedioExecucaoServicoUseCase(
            servicoCatalogoRepository.Object,
            ordemServicoRepository.Object,
            new ConsultarTempoMedioExecucaoServicoValidator(),
            MapperFactory.Criar());
    }
}

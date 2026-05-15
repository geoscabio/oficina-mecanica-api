using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarServicoCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.UnitTests.Administrativo.ServicoCatalogoUseCases.ConsultarServicoCatalogo;

public class ConsultarServicoCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_ServicoCatalogoExistente_Quando_ConsultarServicoCatalogo_Entao_DeveRetornarServicoCatalogo()
    {
        // Arrange
        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        var repository = CriarRepository(servicoCatalogo);

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarConsultarServicoCatalogoRequestValido(
            servicoCatalogo.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(servicoCatalogo.Id);
        resultado.Valor.Descricao.Should().Be(ServicoCatalogoTestDataFactory.DescricaoPadrao);
        resultado.Valor.Valor.Should().Be(ServicoCatalogoTestDataFactory.ValorPadrao);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.ServicoCatalogoId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_ServicoCatalogoInexistente_Quando_ConsultarServicoCatalogo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarConsultarServicoCatalogoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.ServicoCatalogoId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarConsultarServicoCatalogoRequestValido(
            Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IServicoCatalogoRepository> CriarRepository(
        ServicoCatalogo? servicoCatalogo)
    {
        var repository = new Mock<IServicoCatalogoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicoCatalogo);

        return repository;
    }

    private static ConsultarServicoCatalogoUseCase CriarUseCase(
        Mock<IServicoCatalogoRepository> repository)
    {
        return new ConsultarServicoCatalogoUseCase(
            repository.Object,
            new ConsultarServicoCatalogoValidator(),
            MapperFactory.Criar());
    }
}
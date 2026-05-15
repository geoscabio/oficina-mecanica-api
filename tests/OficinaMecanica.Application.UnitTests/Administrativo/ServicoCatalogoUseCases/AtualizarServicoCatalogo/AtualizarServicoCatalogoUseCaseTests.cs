using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.AtualizarServicoCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.UnitTests.Administrativo.ServicoCatalogoUseCases.AtualizarServicoCatalogo;

public class AtualizarServicoCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarServicoCatalogo_Entao_DeveAtualizarServicoCatalogo()
    {
        // Arrange
        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        var repository = CriarRepository(servicoCatalogo);

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarAtualizarServicoCatalogoRequestValido(
            servicoCatalogo.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(servicoCatalogo.Id);
        resultado.Valor.Descricao.Should().Be(request.Descricao);
        resultado.Valor.Valor.Should().Be(request.Valor);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.ServicoCatalogoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<ServicoCatalogo>(servico =>
                    servico.Id == servicoCatalogo.Id
                    && servico.Descricao == request.Descricao
                    && servico.Valor == request.Valor),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_ServicoCatalogoInexistente_Quando_AtualizarServicoCatalogo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarAtualizarServicoCatalogoRequestValido();

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

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<ServicoCatalogo>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_AtualizarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarAtualizarServicoCatalogoRequestValido(
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

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<ServicoCatalogo>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_DescricaoInvalida_Quando_AtualizarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao(
        string descricao)
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarAtualizarServicoCatalogoRequestValido(
            descricao: descricao);

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

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<ServicoCatalogo>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_ValorInvalido_Quando_AtualizarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarAtualizarServicoCatalogoRequestValido(
            valor: 0m);

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

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<ServicoCatalogo>(),
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

    private static AtualizarServicoCatalogoUseCase CriarUseCase(
        Mock<IServicoCatalogoRepository> repository)
    {
        return new AtualizarServicoCatalogoUseCase(
            repository.Object,
            new AtualizarServicoCatalogoValidator(),
            MapperFactory.Criar());
    }
}
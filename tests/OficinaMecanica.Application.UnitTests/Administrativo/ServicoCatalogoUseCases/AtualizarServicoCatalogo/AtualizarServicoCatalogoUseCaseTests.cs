using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.AtualizarServicoCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Builders;
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
        var repository = new Mock<IServicoCatalogoRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(servicoCatalogo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicoCatalogo);

        var useCase = CriarUseCase(repository);
        var request = new AtualizarServicoCatalogoRequest(servicoCatalogo.Id, "Alinhamento", 90m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(servicoCatalogo.Id);
        resultado.Valor.Descricao.Should().Be(request.Descricao);
        resultado.Valor.Valor.Should().Be(request.Valor);
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
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = new AtualizarServicoCatalogoRequest(Guid.NewGuid(), "Alinhamento", 90m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_AtualizarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = new AtualizarServicoCatalogoRequest(Guid.Empty, "Alinhamento", 90m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public async Task Dado_DescricaoVazia_Quando_AtualizarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = new AtualizarServicoCatalogoRequest(Guid.NewGuid(), string.Empty, 90m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public async Task Dado_ValorInvalido_Quando_AtualizarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = new AtualizarServicoCatalogoRequest(Guid.NewGuid(), "Alinhamento", 0m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static AtualizarServicoCatalogoUseCase CriarUseCase(Mock<IServicoCatalogoRepository> repository)
    {
        return new AtualizarServicoCatalogoUseCase(
            repository.Object,
            new AtualizarServicoCatalogoValidator(),
            MapperFactory.Criar());
    }
}

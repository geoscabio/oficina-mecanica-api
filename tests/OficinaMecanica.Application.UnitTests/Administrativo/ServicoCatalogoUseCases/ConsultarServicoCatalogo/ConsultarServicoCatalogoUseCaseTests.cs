using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarServicoCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
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
        var repository = new Mock<IServicoCatalogoRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(servicoCatalogo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicoCatalogo);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarServicoCatalogoRequest(servicoCatalogo.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(servicoCatalogo.Id);
        resultado.Valor.Descricao.Should().Be(servicoCatalogo.Descricao);
        resultado.Valor.Valor.Should().Be(servicoCatalogo.Valor);
    }

    [Fact]
    public async Task Dado_ServicoCatalogoInexistente_Quando_ConsultarServicoCatalogo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarServicoCatalogoRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarServicoCatalogoRequest(Guid.Empty));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static ConsultarServicoCatalogoUseCase CriarUseCase(Mock<IServicoCatalogoRepository> repository)
    {
        return new ConsultarServicoCatalogoUseCase(
            repository.Object,
            new ConsultarServicoCatalogoValidator(),
            MapperFactory.Criar());
    }
}

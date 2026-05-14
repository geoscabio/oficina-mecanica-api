using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ConsultarMecanico;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Builders;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.UnitTests.Administrativo.MecanicoUseCases.ConsultarMecanico;

public class ConsultarMecanicoUseCaseTests
{
    [Fact]
    public async Task Dado_MecanicoExistente_Quando_ConsultarMecanico_Entao_DeveRetornarMecanico()
    {
        // Arrange
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();
        var repository = new Mock<IMecanicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(mecanico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mecanico);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarMecanicoRequest(mecanico.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(mecanico.Id);
        resultado.Valor.Nome.Should().Be(mecanico.Nome);
        resultado.Valor.Funcional.Should().Be(mecanico.Funcional);
    }

    [Fact]
    public async Task Dado_MecanicoInexistente_Quando_ConsultarMecanico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarMecanicoRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(MecanicoErrorMessages.MecanicoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarMecanico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarMecanicoRequest(Guid.Empty));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static ConsultarMecanicoUseCase CriarUseCase(Mock<IMecanicoRepository> repository)
    {
        return new ConsultarMecanicoUseCase(
            repository.Object,
            new ConsultarMecanicoValidator(),
            MapperFactory.Criar());
    }
}
using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ConsultarMecanico;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
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

        var repository = CriarRepository(mecanico);

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarConsultarMecanicoRequestValido(mecanico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(mecanico.Id);
        resultado.Valor.Nome.Should().Be(MecanicoTestDataFactory.NomePadrao);
        resultado.Valor.Funcional.Should().Be(MecanicoTestDataFactory.FuncionalPadrao);

        repository.Verify(repo => repo.ObterPorIdAsync(request.MecanicoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_MecanicoInexistente_Quando_ConsultarMecanico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarConsultarMecanicoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(MecanicoErrorMessages.MecanicoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(repo => repo.ObterPorIdAsync(request.MecanicoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarMecanico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarConsultarMecanicoRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IMecanicoRepository> CriarRepository(Mecanico? mecanico)
    {
        var repository = new Mock<IMecanicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mecanico);

        return repository;
    }

    private static ConsultarMecanicoUseCase CriarUseCase(Mock<IMecanicoRepository> repository)
    {
        return new ConsultarMecanicoUseCase(repository.Object, new ConsultarMecanicoValidator(), MapperFactory.Criar());
    }
}
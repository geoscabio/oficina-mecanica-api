using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ListarMecanicos;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Administrativo.MecanicoUseCases.ListarMecanicos;

public class ListarMecanicosUseCaseTests
{
    [Fact]
    public async Task Dado_MecanicosExistentes_Quando_ListarMecanicos_Entao_DeveRetornarMecanicos()
    {
        // Arrange
        var mecanicos = new[]
        {
            MecanicoTestDataFactory.CriarMecanicoPadrao(),
            MecanicoTestDataFactory.CriarMecanicoPadrao()
        };

        var repository = CriarRepository(mecanicos, totalItens: mecanicos.Length);

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarListarMecanicosRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(MecanicoTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(MecanicoTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(mecanicos.Length);
        resultado.Valor.Itens.Should().HaveCount(mecanicos.Length);
        resultado.Valor.Itens.Select(mecanico => mecanico.Id).Should().BeEquivalentTo(
            mecanicos.Select(mecanico => mecanico.Id));

        repository.Verify(
            repo => repo.ListarAsync(
                MecanicoTestDataFactory.PaginaPadrao,
                MecanicoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_NenhumMecanico_Quando_ListarMecanicos_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var repository = CriarRepository(Array.Empty<Mecanico>(), totalItens: 0);

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarListarMecanicosRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(MecanicoTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(MecanicoTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(0);

        repository.Verify(
            repo => repo.ListarAsync(
                MecanicoTestDataFactory.PaginaPadrao,
                MecanicoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarMecanicos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarListarMecanicosRequestValido(
            pagina: 0);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ListarAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarMecanicos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarListarMecanicosRequestValido(
            tamanhoPagina: 101);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ListarAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IMecanicoRepository> CriarRepository(
        IReadOnlyCollection<Mecanico> mecanicos,
        int totalItens)
    {
        var repository = new Mock<IMecanicoRepository>();

        repository
            .Setup(repo => repo.ListarAsync(
                MecanicoTestDataFactory.PaginaPadrao,
                MecanicoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mecanicos);

        repository
            .Setup(repo => repo.ContarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalItens);

        return repository;
    }

    private static ListarMecanicosUseCase CriarUseCase(Mock<IMecanicoRepository> repository)
    {
        return new ListarMecanicosUseCase(
            repository.Object,
            new ListarMecanicosValidator(),
            MapperFactory.Criar());
    }
}
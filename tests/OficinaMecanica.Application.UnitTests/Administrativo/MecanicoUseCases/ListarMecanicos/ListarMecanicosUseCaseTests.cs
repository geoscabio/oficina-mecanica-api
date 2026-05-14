using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ListarMecanicos;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Builders;
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

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarMecanicosRequest());

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(1);
        resultado.Valor.TamanhoPagina.Should().Be(10);
        resultado.Valor.TotalItens.Should().Be(mecanicos.Length);
        resultado.Valor.Itens.Should().HaveCount(mecanicos.Length);
        resultado.Valor.Itens.Select(mecanico => mecanico.Id).Should().BeEquivalentTo(
            mecanicos.Select(mecanico => mecanico.Id));
    }

    [Fact]
    public async Task Dado_NenhumMecanico_Quando_ListarMecanicos_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var repository = CriarRepository(Array.Empty<Mecanico>(), totalItens: 0);
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarMecanicosRequest());

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.TotalItens.Should().Be(0);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarMecanicos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarMecanicosRequest(Pagina: 0));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarMecanicos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarMecanicosRequest(TamanhoPagina: 101));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static Mock<IMecanicoRepository> CriarRepository(
        IReadOnlyCollection<Mecanico> mecanicos,
        int totalItens)
    {
        var repository = new Mock<IMecanicoRepository>();

        repository
            .Setup(repo => repo.ListarAsync(1, 10, It.IsAny<CancellationToken>()))
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
using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ListarVeiculos;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.ListarVeiculos;

public class ListarVeiculosUseCaseTests
{
    [Fact]
    public async Task Dado_VeiculosExistentes_Quando_ListarVeiculos_Entao_DeveRetornarVeiculos()
    {
        // Arrange
        var veiculos = new[]
        {
            VeiculoTestDataFactory.CriarVeiculoPadrao(),
            VeiculoTestDataFactory.CriarVeiculoPadrao()
        };

        var repository = CriarRepository(veiculos, totalItens: 2);

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarListarVeiculosRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        resultado.Valor.Should().NotBeNull();

        resultado.Valor!.Pagina.Should().Be(VeiculoTestDataFactory.PaginaPadrao);

        resultado.Valor.TamanhoPagina.Should().Be(VeiculoTestDataFactory.TamanhoPaginaPadrao);

        resultado.Valor.TotalItens.Should().Be(2);

        resultado.Valor.Itens.Should().HaveCount(2);

        resultado.Valor.Itens.Select(veiculo => veiculo.Id).Should().BeEquivalentTo(
            veiculos.Select(veiculo => veiculo.Id));

        repository.Verify(
            repo => repo.ListarAsync(
                VeiculoTestDataFactory.PaginaPadrao,
                VeiculoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_NenhumVeiculo_Quando_ListarVeiculos_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var repository = CriarRepository(Array.Empty<Veiculo>(), totalItens: 0);

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarListarVeiculosRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        resultado.Valor.Should().NotBeNull();

        resultado.Valor!.Itens.Should().BeEmpty();

        resultado.Valor.Pagina.Should().Be(VeiculoTestDataFactory.PaginaPadrao);

        resultado.Valor.TamanhoPagina.Should().Be(VeiculoTestDataFactory.TamanhoPaginaPadrao);

        resultado.Valor.TotalItens.Should().Be(0);

        repository.Verify(
            repo => repo.ListarAsync(
                VeiculoTestDataFactory.PaginaPadrao,
                VeiculoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarVeiculos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IVeiculoRepository>();

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarListarVeiculosRequestValido(
            pagina: 0);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

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
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarVeiculos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IVeiculoRepository>();

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarListarVeiculosRequestValido(
            tamanhoPagina: 101);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

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

    private static Mock<IVeiculoRepository> CriarRepository(
        IReadOnlyCollection<Veiculo> veiculos,
        int totalItens)
    {
        var repository = new Mock<IVeiculoRepository>();

        repository
            .Setup(repo => repo.ListarAsync(
                VeiculoTestDataFactory.PaginaPadrao,
                VeiculoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculos);

        repository
            .Setup(repo => repo.ContarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalItens);

        return repository;
    }

    private static ListarVeiculosUseCase CriarUseCase(Mock<IVeiculoRepository> repository)
    {
        return new ListarVeiculosUseCase(
            repository.Object,
            new ListarVeiculosValidator(),
            MapperFactory.Criar());
    }
}
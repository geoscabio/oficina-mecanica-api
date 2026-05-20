using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarServicosCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Administrativo.ServicoCatalogoUseCases.ListarServicosCatalogo;

public class ListarServicosCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_ServicosCatalogoExistentes_Quando_ListarServicosCatalogo_Entao_DeveRetornarServicosCatalogo()
    {
        // Arrange
        var servicosCatalogo = new[]
        {
            ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao(),
            ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao(ServicoCatalogoTestDataFactory.DescricaoAtualizada, ServicoCatalogoTestDataFactory.ValorAtualizado)
        };

        var repository = CriarRepository(servicosCatalogo, totalItens: servicosCatalogo.Length);

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarListarServicosCatalogoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(ServicoCatalogoTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(ServicoCatalogoTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(servicosCatalogo.Length);
        resultado.Valor.Itens.Should().HaveCount(servicosCatalogo.Length);
        resultado.Valor.Itens.Select(servicoCatalogo => servicoCatalogo.Id).Should().BeEquivalentTo(servicosCatalogo.Select(servicoCatalogo => servicoCatalogo.Id));

        repository.Verify(repo => repo.ListarAsync(ServicoCatalogoTestDataFactory.PaginaPadrao, ServicoCatalogoTestDataFactory.TamanhoPaginaPadrao, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_NenhumServicoCatalogo_Quando_ListarServicosCatalogo_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var repository = CriarRepository(Array.Empty<ServicoCatalogo>(), totalItens: 0);

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarListarServicosCatalogoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(ServicoCatalogoTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(ServicoCatalogoTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(0);

        repository.Verify(repo => repo.ListarAsync(ServicoCatalogoTestDataFactory.PaginaPadrao, ServicoCatalogoTestDataFactory.TamanhoPaginaPadrao, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarServicosCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarListarServicosCatalogoRequestValido(pagina: 0);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ListarAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarServicosCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarListarServicosCatalogoRequestValido(tamanhoPagina: 101);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ListarAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IServicoCatalogoRepository> CriarRepository(IReadOnlyCollection<ServicoCatalogo> servicosCatalogo, int totalItens)
    {
        var repository = new Mock<IServicoCatalogoRepository>();

        repository
            .Setup(repo => repo.ListarAsync(ServicoCatalogoTestDataFactory.PaginaPadrao, ServicoCatalogoTestDataFactory.TamanhoPaginaPadrao, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicosCatalogo);

        repository
            .Setup(repo => repo.ContarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalItens);

        return repository;
    }

    private static ListarServicosCatalogoUseCase CriarUseCase(Mock<IServicoCatalogoRepository> repository)
    {
        return new ListarServicosCatalogoUseCase(repository.Object, new ListarServicosCatalogoValidator(), MapperFactory.Criar());
    }
}
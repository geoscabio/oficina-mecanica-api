using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ListarClientes;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.ListarClientes;

public class ListarClientesUseCaseTests
{
    [Fact]
    public async Task Dado_ClientesExistentes_Quando_ListarClientes_Entao_DeveRetornarClientes()
    {
        // Arrange
        var clientes = new[]
        {
            ClienteTestDataFactory.CriarClientePadrao(),
            ClienteTestDataFactory.CriarClientePadrao()
        };

        var repository = CriarRepository(clientes, totalItens: 2);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarListarClientesRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(ClienteTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(ClienteTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(2);
        resultado.Valor.Itens.Should().HaveCount(2);
        resultado.Valor.Itens.Select(cliente => cliente.Id).Should().BeEquivalentTo(clientes.Select(cliente => cliente.Id));

        repository.Verify(repo => repo.ListarAsync(ClienteTestDataFactory.PaginaPadrao, ClienteTestDataFactory.TamanhoPaginaPadrao, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_NenhumCliente_Quando_ListarClientes_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var repository = CriarRepository(Array.Empty<Cliente>(), totalItens: 0);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarListarClientesRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(ClienteTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(ClienteTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(0);

        repository.Verify(repo => repo.ListarAsync(ClienteTestDataFactory.PaginaPadrao, ClienteTestDataFactory.TamanhoPaginaPadrao, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarClientes_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarListarClientesRequestValido(pagina: 0);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ListarAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarClientes_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarListarClientesRequestValido(tamanhoPagina: 101);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ListarAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IClienteRepository> CriarRepository(IReadOnlyCollection<Cliente> clientes, int totalItens)
    {
        var repository = new Mock<IClienteRepository>();

        repository
            .Setup(repo => repo.ListarAsync(ClienteTestDataFactory.PaginaPadrao, ClienteTestDataFactory.TamanhoPaginaPadrao, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientes);

        repository
            .Setup(repo => repo.ContarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalItens);

        return repository;
    }

    private static ListarClientesUseCase CriarUseCase(Mock<IClienteRepository> repository)
    {
        return new ListarClientesUseCase(repository.Object, new ListarClientesValidator(), MapperFactory.Criar());
    }
}
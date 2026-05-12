using FluentAssertions;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;
using Moq;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.CadastrarVeiculo;

public class CadastrarVeiculoUseCaseTests
{
    [Fact]
    public async Task Dado_RequestValido_Quando_CadastrarVeiculo_Entao_DevePersistirVeiculoERetornarSucesso()
    {
        var cliente = CriarCliente();
        var clienteRepository = new Mock<IClienteRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        clienteRepository
            .Setup(repo => repo.ObterPorIdAsync(cliente.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);
        veiculoRepository
            .Setup(repo => repo.ObterPorPlacaAsync("ABC1234", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Veiculo?)null);
        var useCase = new CadastrarVeiculoUseCase(
            veiculoRepository.Object,
            clienteRepository.Object,
            new CadastrarVeiculoValidator(),
            MapperFactory.Criar());
        var request = CriarRequestValido(cliente.Id);

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.ClienteId.Should().Be(cliente.Id);
        resultado.Valor.Placa.Should().Be("ABC1234");
        resultado.Valor.Marca.Should().Be("Toyota");
        resultado.Valor.Modelo.Should().Be("Corolla");
        resultado.Valor.Ano.Should().Be(2020);
        veiculoRepository.Verify(repo => repo.AdicionarAsync(It.Is<Veiculo>(veiculo =>
            veiculo.ClienteId == cliente.Id
            && veiculo.Placa.NumeroPlaca == "ABC1234"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_ClienteInexistente_Quando_CadastrarVeiculo_Entao_DeveRetornarFalha()
    {
        var clienteRepository = new Mock<IClienteRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var useCase = new CadastrarVeiculoUseCase(
            veiculoRepository.Object,
            clienteRepository.Object,
            new CadastrarVeiculoValidator(),
            MapperFactory.Criar());
        var request = CriarRequestValido(Guid.NewGuid());

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Cliente nao encontrado.");
        veiculoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_PlacaJaCadastrada_Quando_CadastrarVeiculo_Entao_DeveRetornarFalha()
    {
        var cliente = CriarCliente();
        var veiculoExistente = CriarVeiculo(cliente.Id);
        var clienteRepository = new Mock<IClienteRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        clienteRepository
            .Setup(repo => repo.ObterPorIdAsync(cliente.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);
        veiculoRepository
            .Setup(repo => repo.ObterPorPlacaAsync("ABC1234", It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculoExistente);
        var useCase = new CadastrarVeiculoUseCase(
            veiculoRepository.Object,
            clienteRepository.Object,
            new CadastrarVeiculoValidator(),
            MapperFactory.Criar());
        var request = CriarRequestValido(cliente.Id);

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Veiculo ja cadastrado para a placa informada.");
        veiculoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_ClienteIdVazio_Quando_CadastrarVeiculo_Entao_DeveLancarValidationException()
    {
        var clienteRepository = new Mock<IClienteRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var useCase = new CadastrarVeiculoUseCase(
            veiculoRepository.Object,
            clienteRepository.Object,
            new CadastrarVeiculoValidator(),
            MapperFactory.Criar());
        var request = CriarRequestValido(Guid.Empty);

        var acao = () => useCase.ExecuteAsync(request);

        await acao.Should().ThrowAsync<ValidationException>();
    }

    private static CadastrarVeiculoRequest CriarRequestValido(Guid clienteId)
    {
        return new CadastrarVeiculoRequest(
            ClienteId: clienteId,
            Placa: "ABC-1234",
            Marca: "Toyota",
            Modelo: "Corolla",
            Ano: 2020);
    }

    private static Cliente CriarCliente()
    {
        return Cliente.Criar(
            CpfCnpj.Criar("529.982.247-25"),
            "Maria Silva",
            new Endereco("Rua A", "100", "Centro", "Sao Paulo", "01001000"),
            Telefone.Criar("(11) 99999-9999"),
            Email.Criar("maria@email.com"));
    }

    private static Veiculo CriarVeiculo(Guid clienteId)
    {
        return Veiculo.Criar(
            clienteId,
            Placa.Criar("ABC-1234"),
            "Toyota",
            "Corolla",
            2020);
    }
}

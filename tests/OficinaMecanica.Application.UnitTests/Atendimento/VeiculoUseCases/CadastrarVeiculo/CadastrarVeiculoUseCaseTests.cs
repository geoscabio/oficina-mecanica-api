using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Builders;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.CadastrarVeiculo;

public class CadastrarVeiculoUseCaseTests
{
    [Fact]
    public async Task Dado_RequestValido_Quando_CadastrarVeiculo_Entao_DevePersistirVeiculoERetornarSucesso()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var clienteRepository = new Mock<IClienteRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();

        clienteRepository
            .Setup(repo => repo.ObterPorIdAsync(cliente.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        veiculoRepository
            .Setup(repo => repo.ObterPorPlacaAsync("ABC1234", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Veiculo?)null);

        var useCase = CriarUseCase(veiculoRepository, clienteRepository);
        var request = CriarRequestValido(cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
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
        // Arrange
        var clienteRepository = new Mock<IClienteRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var useCase = CriarUseCase(veiculoRepository, clienteRepository);
        var request = CriarRequestValido(Guid.NewGuid());

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(ClienteErrorMessages.ClienteNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        veiculoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_PlacaJaCadastrada_Quando_CadastrarVeiculo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var veiculoExistente = VeiculoTestDataFactory.CriarVeiculoPadrao(cliente.Id);
        var clienteRepository = new Mock<IClienteRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();

        clienteRepository
            .Setup(repo => repo.ObterPorIdAsync(cliente.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        veiculoRepository
            .Setup(repo => repo.ObterPorPlacaAsync("ABC1234", It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculoExistente);

        var useCase = CriarUseCase(veiculoRepository, clienteRepository);
        var request = CriarRequestValido(cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(VeiculoErrorMessages.VeiculoDuplicado);
        resultado.Erro.Tipo.Should().Be(TipoErro.RegraNegocio);

        veiculoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_ClienteIdVazio_Quando_CadastrarVeiculo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var clienteRepository = new Mock<IClienteRepository>();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var useCase = CriarUseCase(veiculoRepository, clienteRepository);
        var request = CriarRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static CadastrarVeiculoUseCase CriarUseCase(
        Mock<IVeiculoRepository> veiculoRepository,
        Mock<IClienteRepository> clienteRepository)
    {
        return new CadastrarVeiculoUseCase(
            veiculoRepository.Object,
            clienteRepository.Object,
            new CadastrarVeiculoValidator(),
            MapperFactory.Criar());
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
}








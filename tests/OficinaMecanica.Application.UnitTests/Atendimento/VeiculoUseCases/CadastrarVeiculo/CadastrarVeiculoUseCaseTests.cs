using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.CadastrarVeiculo;

public class CadastrarVeiculoUseCaseTests
{
    [Fact]
    public async Task Dado_RequestValido_Quando_CadastrarVeiculo_Entao_DevePersistirVeiculoERetornarSucesso()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();

        var clienteRepository = CriarClienteRepository(cliente);

        var veiculoRepository = CriarVeiculoRepository(null);

        var useCase = CriarUseCase(veiculoRepository, clienteRepository);

        var request = VeiculoTestDataFactory.CriarCadastrarVeiculoRequestValido(cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.ClienteId.Should().Be(cliente.Id);
        resultado.Valor.Placa.Should().Be(VeiculoTestDataFactory.PlacaNormalizadaPadrao);
        resultado.Valor.Marca.Should().Be(VeiculoTestDataFactory.MarcaPadrao);
        resultado.Valor.Modelo.Should().Be(VeiculoTestDataFactory.ModeloPadrao);
        resultado.Valor.Ano.Should().Be(VeiculoTestDataFactory.AnoPadrao);

        clienteRepository.Verify(repo => repo.ObterPorIdAsync(request.ClienteId, It.IsAny<CancellationToken>()), Times.Once);

        veiculoRepository.Verify(repo => repo.ObterPorPlacaAsync(VeiculoTestDataFactory.PlacaNormalizadaPadrao, It.IsAny<CancellationToken>()), Times.Once);

        veiculoRepository.Verify(
            repo => repo.AdicionarAsync(
                It.Is<Veiculo>(veiculo =>
                    veiculo.ClienteId == cliente.Id
                    && veiculo.Placa.NumeroPlaca == VeiculoTestDataFactory.PlacaNormalizadaPadrao
                    && veiculo.Marca == VeiculoTestDataFactory.MarcaPadrao
                    && veiculo.Modelo == VeiculoTestDataFactory.ModeloPadrao
                    && veiculo.Ano == VeiculoTestDataFactory.AnoPadrao),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_ClienteInexistente_Quando_CadastrarVeiculo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var clienteRepository = CriarClienteRepository(null);

        var veiculoRepository = new Mock<IVeiculoRepository>();

        var useCase = CriarUseCase(veiculoRepository, clienteRepository);

        var request = VeiculoTestDataFactory.CriarCadastrarVeiculoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(ClienteErrorMessages.ClienteNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        clienteRepository.Verify(repo => repo.ObterPorIdAsync(request.ClienteId, It.IsAny<CancellationToken>()), Times.Once);

        veiculoRepository.Verify(repo => repo.ObterPorPlacaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        veiculoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_PlacaJaCadastrada_Quando_CadastrarVeiculo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();

        var veiculoExistente = VeiculoTestDataFactory.CriarVeiculoPadrao(cliente.Id);

        var clienteRepository = CriarClienteRepository(cliente);

        var veiculoRepository = CriarVeiculoRepository(veiculoExistente);

        var useCase = CriarUseCase(veiculoRepository, clienteRepository);

        var request = VeiculoTestDataFactory.CriarCadastrarVeiculoRequestValido(cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(VeiculoErrorMessages.VeiculoDuplicado);
        resultado.Erro.Tipo.Should().Be(TipoErro.RegraNegocio);

        clienteRepository.Verify(repo => repo.ObterPorIdAsync(request.ClienteId, It.IsAny<CancellationToken>()), Times.Once);

        veiculoRepository.Verify(repo => repo.ObterPorPlacaAsync(VeiculoTestDataFactory.PlacaNormalizadaPadrao, It.IsAny<CancellationToken>()), Times.Once);

        veiculoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_ClienteIdVazio_Quando_CadastrarVeiculo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var clienteRepository = new Mock<IClienteRepository>();

        var veiculoRepository = new Mock<IVeiculoRepository>();

        var useCase = CriarUseCase(veiculoRepository, clienteRepository);

        var request = VeiculoTestDataFactory.CriarCadastrarVeiculoRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        clienteRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        veiculoRepository.Verify(repo => repo.ObterPorPlacaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        veiculoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IClienteRepository> CriarClienteRepository(Cliente? cliente)
    {
        var repository = new Mock<IClienteRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        return repository;
    }

    private static Mock<IVeiculoRepository> CriarVeiculoRepository(Veiculo? veiculo)
    {
        var repository = new Mock<IVeiculoRepository>();

        repository
            .Setup(repo => repo.ObterPorPlacaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        return repository;
    }

    private static CadastrarVeiculoUseCase CriarUseCase(Mock<IVeiculoRepository> veiculoRepository, Mock<IClienteRepository> clienteRepository)
    {
        return new CadastrarVeiculoUseCase(veiculoRepository.Object, clienteRepository.Object, new CadastrarVeiculoValidator(), MapperFactory.Criar());
    }
}
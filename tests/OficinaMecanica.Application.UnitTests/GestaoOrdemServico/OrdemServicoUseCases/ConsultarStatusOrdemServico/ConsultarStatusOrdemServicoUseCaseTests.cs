using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarStatusOrdemServico;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Builders;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.ConsultarStatusOrdemServico;

public class ConsultarStatusOrdemServicoUseCaseTests
{
    [Fact]
    public async Task Dado_OrdemServicoExistente_Quando_ConsultarStatusOrdemServico_Entao_DeveRetornarStatusDaOrdemServico()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoEmExecucao();
        var servico = ordemServico.Servicos.Single();
        var repository = new Mock<IOrdemServicoRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarStatusOrdemServicoRequest(ordemServico.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.OrdemServicoId.Should().Be(ordemServico.Id);
        resultado.Valor.Numero.Should().Be(ordemServico.Numero);
        resultado.Valor.Status.Should().Be("EM_EXECUCAO");
        resultado.Valor.Servicos.Should().ContainSingle();
        resultado.Valor.Servicos.Single().ServicoId.Should().Be(servico.Id);
        resultado.Valor.Servicos.Single().ServicoCatalogoId.Should().Be(servico.ServicoCatalogoId);
        resultado.Valor.Servicos.Single().Status.Should().Be("EM_EXECUCAO");
    }

    [Fact]
    public async Task Dado_OrdemServicoSemServicos_Quando_ConsultarStatusOrdemServico_Entao_DeveRetornarStatusSemServicos()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoRecebida();
        var repository = new Mock<IOrdemServicoRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarStatusOrdemServicoRequest(ordemServico.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Status.Should().Be("RECEBIDA");
        resultado.Valor.Servicos.Should().BeEmpty();
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_ConsultarStatusOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarStatusOrdemServicoRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarStatusOrdemServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarStatusOrdemServicoRequest(Guid.Empty));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static ConsultarStatusOrdemServicoUseCase CriarUseCase(Mock<IOrdemServicoRepository> repository)
    {
        return new ConsultarStatusOrdemServicoUseCase(
            repository.Object,
            new ConsultarStatusOrdemServicoValidator());
    }
}

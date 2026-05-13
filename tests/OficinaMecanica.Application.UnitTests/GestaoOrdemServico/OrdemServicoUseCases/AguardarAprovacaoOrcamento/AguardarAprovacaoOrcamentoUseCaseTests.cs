using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AguardarAprovacaoOrcamento;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Builders;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.AguardarAprovacaoOrcamento;

public class AguardarAprovacaoOrcamentoUseCaseTests
{
    [Fact]
    public async Task Dado_OrdemServicoEmDiagnosticoComServicoDefinido_Quando_AguardarAprovacaoOrcamento_Entao_DeveAtualizarStatusEPersistir()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnosticoComServico();
        var repository = new Mock<IOrdemServicoRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new AguardarAprovacaoOrcamentoRequest(ordemServico.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(ordemServico.Id);
        resultado.Valor.Status.Should().Be("AGUARDANDO_APROVACAO");
        resultado.Valor.ValorTotal.Should().Be(150m);

        repository.Verify(repo => repo.AtualizarAsync(It.Is<OrdemServico>(ordemServicoAtualizada =>
            ordemServicoAtualizada.Id == ordemServico.Id
            && ordemServicoAtualizada.Status.ToString() == "AGUARDANDO_APROVACAO"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_AguardarAprovacaoOrcamento_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new AguardarAprovacaoOrcamentoRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact]
    public async Task Dado_OrdemServicoEmDiagnosticoSemServico_Quando_AguardarAprovacaoOrcamento_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();
        var repository = new Mock<IOrdemServicoRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var useCase = CriarUseCase(repository);

        // Act
        var acao = () => useCase.ExecuteAsync(new AguardarAprovacaoOrcamentoRequest(ordemServico.Id));

        // Assert
        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicoObrigatorioParaAguardarAprovacao);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_AguardarAprovacaoOrcamento_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new AguardarAprovacaoOrcamentoRequest(Guid.Empty));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static AguardarAprovacaoOrcamentoUseCase CriarUseCase(Mock<IOrdemServicoRepository> repository)
    {
        return new AguardarAprovacaoOrcamentoUseCase(
            repository.Object,
            new AguardarAprovacaoOrcamentoValidator(),
            MapperFactory.Criar());
    }
}

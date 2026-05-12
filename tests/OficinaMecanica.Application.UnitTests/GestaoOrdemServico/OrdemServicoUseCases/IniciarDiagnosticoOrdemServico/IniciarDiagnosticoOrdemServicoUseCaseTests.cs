using FluentAssertions;
using FluentValidation;
using Moq;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;

public class IniciarDiagnosticoOrdemServicoUseCaseTests
{
    [Fact]
    public async Task Dado_OrdemServicoRecebida_Quando_IniciarDiagnostico_Entao_DeveAtualizarStatusEPersistir()
    {
        var ordemServico = OrdemServico.Abrir(Guid.NewGuid(), Guid.NewGuid());
        var repository = new Mock<IOrdemServicoRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);
        var useCase = CriarUseCase(repository);

        var resultado = await useCase.ExecuteAsync(new IniciarDiagnosticoOrdemServicoRequest(ordemServico.Id));

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(ordemServico.Id);
        resultado.Valor.Status.Should().Be("EM_DIAGNOSTICO");
        repository.Verify(repo => repo.AtualizarAsync(It.Is<OrdemServico>(ordemServicoAtualizada =>
            ordemServicoAtualizada.Id == ordemServico.Id
            && ordemServicoAtualizada.Status.ToString() == "EM_DIAGNOSTICO"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_IniciarDiagnostico_Entao_DeveRetornarFalha()
    {
        var repository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(repository);

        var resultado = await useCase.ExecuteAsync(new IniciarDiagnosticoOrdemServicoRequest(Guid.NewGuid()));

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Ordem de servico nao encontrada.");
        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_IniciarDiagnostico_Entao_DeveLancarValidationException()
    {
        var repository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(repository);

        var acao = () => useCase.ExecuteAsync(new IniciarDiagnosticoOrdemServicoRequest(Guid.Empty));

        await acao.Should().ThrowAsync<ValidationException>();
    }

    private static IniciarDiagnosticoOrdemServicoUseCase CriarUseCase(Mock<IOrdemServicoRepository> repository)
    {
        return new IniciarDiagnosticoOrdemServicoUseCase(
            repository.Object,
            new IniciarDiagnosticoOrdemServicoValidator(),
            MapperFactory.Criar());
    }
}

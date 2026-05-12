using FluentAssertions;
using OficinaMecanica.Domain.GestaoOrdemServico.Entities;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.GestaoOrdemServico.Exceptions;

namespace OficinaMecanica.Domain.UnitTests.GestaoOrdemServico;

public class ServicoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarServico_Entao_DeveRegistrarServicoPendenteDeExecucao()
    {
        var servicoCatalogoId = Guid.NewGuid();

        var servico = Servico.Criar(servicoCatalogoId, 150m);

        servico.Id.Should().NotBeEmpty();
        servico.ServicoCatalogoId.Should().Be(servicoCatalogoId);
        servico.Valor.Should().Be(150m);
        servico.Status.Should().Be(StatusServico.PENDENTE);
        servico.DataInicio.Should().BeNull();
        servico.DataFim.Should().BeNull();
    }

    [Fact]
    public void Dado_ServicoPendente_Quando_IniciarExecucao_Entao_DeveFicarEmExecucao()
    {
        var servico = Servico.Criar(Guid.NewGuid(), 150m);

        servico.IniciarExecucao();

        servico.Status.Should().Be(StatusServico.EM_EXECUCAO);
        servico.DataInicio.Should().NotBeNull();
    }

    [Fact]
    public void Dado_ServicoEmExecucao_Quando_Finalizar_Entao_DeveFicarFinalizado()
    {
        var servico = Servico.Criar(Guid.NewGuid(), 150m);
        servico.IniciarExecucao();

        servico.Finalizar();

        servico.Status.Should().Be(StatusServico.FINALIZADO);
        servico.DataFim.Should().NotBeNull();
    }

    [Fact]
    public void Dado_ServicoPendente_Quando_Finalizar_Entao_DeveLancarServicoInvalidoException()
    {
        var servico = Servico.Criar(Guid.NewGuid(), 150m);

        var acao = servico.Finalizar;

        acao.Should().Throw<ServicoInvalidoException>();
    }
}

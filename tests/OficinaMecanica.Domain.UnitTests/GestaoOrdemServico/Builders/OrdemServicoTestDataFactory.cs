using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Entities;

namespace OficinaMecanica.Domain.UnitTests.GestaoOrdemServico.Builders;

internal static class OrdemServicoTestDataFactory
{
    public const decimal ValorServicoPadrao = 150m;
    public const decimal ValorPecaInsumoPadrao = 45m;
    public const int QuantidadePecaInsumoPadrao = 2;

    public static OrdemServico CriarOrdemServicoPadrao()
    {
        return OrdemServico.Abrir(Guid.NewGuid(), Guid.NewGuid());
    }

    public static OrdemServico CriarOrdemServicoEmDiagnostico()
    {
        var ordemServico = CriarOrdemServicoPadrao();
        ordemServico.IniciarDiagnostico();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoAguardandoAprovacao()
    {
        var ordemServico = CriarOrdemServicoEmDiagnostico();
        ordemServico.DefinirServico(Guid.NewGuid(), ValorServicoPadrao);
        ordemServico.AguardarAprovacao();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucao()
    {
        var ordemServico = CriarOrdemServicoAguardandoAprovacao();
        ordemServico.IniciarExecucao();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoFinalizada()
    {
        var ordemServico = CriarOrdemServicoEmExecucao();
        var servicoId = ordemServico.Servicos.Single().Id;

        ordemServico.IniciarExecucaoServico(servicoId);
        ordemServico.FinalizarServico(servicoId);
        ordemServico.Finalizar();

        return ordemServico;
    }

    public static Servico CriarServicoPadrao(Guid? servicoCatalogoId = null)
    {
        return Servico.Criar(servicoCatalogoId ?? Guid.NewGuid(), ValorServicoPadrao);
    }

    public static Servico CriarServicoEmExecucao()
    {
        var servico = CriarServicoPadrao();
        servico.IniciarExecucao();

        return servico;
    }

    public static PecaInsumo CriarPecaInsumoPadrao(Guid? pecaInsumoCatalogoId = null)
    {
        return PecaInsumo.Criar(
            pecaInsumoCatalogoId ?? Guid.NewGuid(),
            QuantidadePecaInsumoPadrao,
            ValorPecaInsumoPadrao);
    }
}

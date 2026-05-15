using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;

internal static class OrdemServicoTestDataFactory
{
    public static OrdemServico CriarOrdemServicoRecebida()
    {
        return OrdemServico.Abrir(1, Guid.NewGuid(), Guid.NewGuid());
    }

    public static OrdemServico CriarOrdemServicoEmDiagnostico()
    {
        var ordemServico = CriarOrdemServicoRecebida();

        ordemServico.IniciarDiagnostico();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmDiagnosticoComServico()
    {
        var ordemServico = CriarOrdemServicoEmDiagnostico();

        ordemServico.DefinirServico(Guid.NewGuid(), 150m);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmDiagnosticoComOrcamentoCompleto()
    {
        var ordemServico = CriarOrdemServicoEmDiagnosticoComServico();

        ordemServico.ReservarPecaInsumo(Guid.NewGuid(), 2, 45m);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoAguardandoAprovacao()
    {
        var ordemServico = CriarOrdemServicoEmDiagnosticoComServico();

        ordemServico.AguardarAprovacao();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucaoComServicoPendente()
    {
        var ordemServico = CriarOrdemServicoAguardandoAprovacao();

        ordemServico.IniciarExecucao();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucaoComServicoEmExecucao()
    {
        var ordemServico = CriarOrdemServicoEmExecucaoComServicoPendente();
        var servicoId = ordemServico.Servicos.Single().Id;

        ordemServico.IniciarExecucaoServico(servicoId);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucaoComServicoFinalizado()
    {
        var ordemServico = CriarOrdemServicoEmExecucaoComServicoEmExecucao();
        var servicoId = ordemServico.Servicos.Single().Id;

        ordemServico.FinalizarServico(servicoId);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoFinalizada()
    {
        var ordemServico = CriarOrdemServicoEmExecucaoComServicoFinalizado();

        ordemServico.Finalizar();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucaoComServicoFinalizadoEPecaInsumoReservado(
        Guid pecaInsumoCatalogoId,
        int quantidade = 2)
    {
        var ordemServico = CriarOrdemServicoEmDiagnosticoComServico();

        ordemServico.ReservarPecaInsumo(pecaInsumoCatalogoId, quantidade, 45m);
        ordemServico.AguardarAprovacao();
        ordemServico.IniciarExecucao();

        var servicoId = ordemServico.Servicos.Single().Id;
        ordemServico.IniciarExecucaoServico(servicoId);
        ordemServico.FinalizarServico(servicoId);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoAguardandoAprovacaoComPecaInsumoReservado(
        Guid pecaInsumoCatalogoId,
        int quantidade = 2)
    {
        var ordemServico = CriarOrdemServicoEmDiagnosticoComServico();

        ordemServico.ReservarPecaInsumo(pecaInsumoCatalogoId, quantidade, 45m);
        ordemServico.AguardarAprovacao();

        return ordemServico;
    }
}


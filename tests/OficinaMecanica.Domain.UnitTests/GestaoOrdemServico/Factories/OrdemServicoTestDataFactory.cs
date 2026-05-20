using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Entities;

namespace OficinaMecanica.Domain.UnitTests.GestaoOrdemServico.Factories;

internal static class OrdemServicoTestDataFactory
{
    public const int NumeroPadrao = 1;

    public const decimal ValorServicoPadrao = 150m;
    public const decimal ValorServicoAtualizado = 80m;

    public const decimal ValorPecaInsumoPadrao = 45m;
    public const int QuantidadePecaInsumoPadrao = 2;

    public static OrdemServico CriarOrdemServicoPadrao(int numero = NumeroPadrao, Guid? veiculoId = null, Guid? mecanicoId = null)
    {
        return OrdemServico.Abrir(numero, veiculoId ?? Guid.NewGuid(), mecanicoId ?? Guid.NewGuid());
    }

    public static OrdemServico CriarOrdemServicoEmDiagnostico()
    {
        var ordemServico = CriarOrdemServicoPadrao();

        ordemServico.IniciarDiagnostico();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmDiagnosticoComServico()
    {
        var ordemServico = CriarOrdemServicoEmDiagnostico();

        ordemServico.DefinirServico(Guid.NewGuid(), ValorServicoPadrao);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmDiagnosticoComServicoEPecaInsumo()
    {
        var ordemServico = CriarOrdemServicoEmDiagnosticoComServico();

        ordemServico.ReservarPecaInsumo(Guid.NewGuid(), QuantidadePecaInsumoPadrao, ValorPecaInsumoPadrao);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoAguardandoAprovacao()
    {
        var ordemServico = CriarOrdemServicoEmDiagnosticoComServico();

        ordemServico.AguardarAprovacao();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucao()
    {
        var ordemServico = CriarOrdemServicoAguardandoAprovacao();

        ordemServico.IniciarExecucao();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucaoComServicoEmExecucao()
    {
        var ordemServico = CriarOrdemServicoEmExecucao();

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

    public static OrdemServico CriarOrdemServicoEntregue()
    {
        var ordemServico = CriarOrdemServicoFinalizada();

        ordemServico.Entregar();

        return ordemServico;
    }

    public static Servico CriarServicoPadrao(Guid? servicoCatalogoId = null, decimal valor = ValorServicoPadrao)
    {
        return Servico.Criar(servicoCatalogoId ?? Guid.NewGuid(), valor);
    }

    public static Servico CriarServicoEmExecucao()
    {
        var servico = CriarServicoPadrao();

        servico.IniciarExecucao();

        return servico;
    }

    public static Servico CriarServicoFinalizado()
    {
        var servico = CriarServicoEmExecucao();

        servico.Finalizar();

        return servico;
    }

    public static PecaInsumo CriarPecaInsumoPadrao(Guid? pecaInsumoCatalogoId = null, int quantidade = QuantidadePecaInsumoPadrao, decimal valorUnitario = ValorPecaInsumoPadrao)
    {
        return PecaInsumo.Criar(pecaInsumoCatalogoId ?? Guid.NewGuid(), quantidade, valorUnitario);
    }
}
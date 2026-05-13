using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Builders;

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
}


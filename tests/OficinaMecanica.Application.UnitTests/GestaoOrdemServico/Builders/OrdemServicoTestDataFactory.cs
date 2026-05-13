using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Builders;

internal static class OrdemServicoTestDataFactory
{
    public static OrdemServico CriarOrdemServicoRecebida()
    {
        return OrdemServico.Abrir(Guid.NewGuid(), Guid.NewGuid());
    }

    public static OrdemServico CriarOrdemServicoEmDiagnostico()
    {
        var ordemServico = CriarOrdemServicoRecebida();

        ordemServico.IniciarDiagnostico();

        return ordemServico;
    }
}


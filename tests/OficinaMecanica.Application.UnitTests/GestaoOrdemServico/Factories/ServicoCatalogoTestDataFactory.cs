using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarTempoMedioExecucaoServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ListarTempoMedioExecucaoServicos;
using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;

internal static class ServicoCatalogoTestDataFactory
{
    public const string DescricaoPadrao = "Troca de oleo";
    public const decimal ValorPadrao = 150m;
    public const double TempoMedioExecucaoPadrao = 120d;

    public const string DescricaoAtualizada = "Alinhamento";
    public const decimal ValorAtualizado = 90m;

    public const int PaginaPadrao = 1;
    public const int TamanhoPaginaPadrao = 10;

    public static ServicoCatalogo CriarServicoCatalogoPadrao(string descricao = DescricaoPadrao, decimal valor = ValorPadrao)
    {
        return ServicoCatalogo.Criar(descricao, valor);
    }

    public static ConsultarTempoMedioExecucaoServicoRequest CriarConsultarTempoMedioExecucaoServicoRequestValido(Guid? servicoCatalogoId = null)
    {
        return new ConsultarTempoMedioExecucaoServicoRequest(servicoCatalogoId ?? Guid.NewGuid());
    }

    public static ListarTempoMedioExecucaoServicosRequest CriarListarTempoMedioExecucaoServicosRequestValido(int pagina = PaginaPadrao, int tamanhoPagina = TamanhoPaginaPadrao)
    {
        return new ListarTempoMedioExecucaoServicosRequest(pagina, tamanhoPagina);
    }
}


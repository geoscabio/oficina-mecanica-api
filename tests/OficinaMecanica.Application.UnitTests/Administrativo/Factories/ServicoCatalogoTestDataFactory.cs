using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.AtualizarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.CadastrarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarTempoMedioExecucaoServico;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarServicosCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarTempoMedioExecucaoServicos;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.RemoverServicoCatalogo;
using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Application.UnitTests.Administrativo.Factories;

internal static class ServicoCatalogoTestDataFactory
{
    public const string DescricaoPadrao = "Troca de oleo";
    public const decimal ValorPadrao = 150m;
    public const double TempoMedioExecucaoPadrao = 120d;

    public const string DescricaoAtualizada = "Alinhamento";
    public const decimal ValorAtualizado = 90m;
    public const double TempoMedioExecucaoAtualizado = 75d;

    public const int PaginaPadrao = 1;
    public const int TamanhoPaginaPadrao = 10;

    public static ServicoCatalogo CriarServicoCatalogoPadrao(
        string descricao = DescricaoPadrao,
        decimal valor = ValorPadrao)
    {
        return ServicoCatalogo.Criar(
            descricao,
            valor);
    }

    public static CadastrarServicoCatalogoRequest CriarCadastrarServicoCatalogoRequestValido(
        string descricao = DescricaoPadrao,
        decimal valor = ValorPadrao)
    {
        return new CadastrarServicoCatalogoRequest(
            descricao,
            valor);
    }

    public static AtualizarServicoCatalogoRequest CriarAtualizarServicoCatalogoRequestValido(
        Guid? servicoCatalogoId = null,
        string descricao = DescricaoAtualizada,
        decimal valor = ValorAtualizado)
    {
        return new AtualizarServicoCatalogoRequest(
            servicoCatalogoId ?? Guid.NewGuid(),
            descricao,
            valor);
    }

    public static ConsultarServicoCatalogoRequest CriarConsultarServicoCatalogoRequestValido(
        Guid? servicoCatalogoId = null)
    {
        return new ConsultarServicoCatalogoRequest(
            servicoCatalogoId ?? Guid.NewGuid());
    }

    public static ConsultarTempoMedioExecucaoServicoRequest CriarConsultarTempoMedioExecucaoServicoRequestValido(
        Guid? servicoCatalogoId = null)
    {
        return new ConsultarTempoMedioExecucaoServicoRequest(
            servicoCatalogoId ?? Guid.NewGuid());
    }

    public static ListarServicosCatalogoRequest CriarListarServicosCatalogoRequestValido(
        int pagina = PaginaPadrao,
        int tamanhoPagina = TamanhoPaginaPadrao)
    {
        return new ListarServicosCatalogoRequest(
            pagina,
            tamanhoPagina);
    }

    public static ListarTempoMedioExecucaoServicosRequest CriarListarTempoMedioExecucaoServicosRequestValido(
        int pagina = PaginaPadrao,
        int tamanhoPagina = TamanhoPaginaPadrao)
    {
        return new ListarTempoMedioExecucaoServicosRequest(
            pagina,
            tamanhoPagina);
    }

    public static RemoverServicoCatalogoRequest CriarRemoverServicoCatalogoRequestValido(
        Guid? servicoCatalogoId = null)
    {
        return new RemoverServicoCatalogoRequest(
            servicoCatalogoId ?? Guid.NewGuid());
    }
}
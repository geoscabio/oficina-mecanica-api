using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.AtualizarPecaInsumoCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ConsultarPecaInsumoCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ListarPecasInsumosCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.RemoverPecaInsumoCatalogo;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;

namespace OficinaMecanica.Application.UnitTests.Administrativo.Factories;

internal static class PecaInsumoCatalogoTestDataFactory
{
    public const string DescricaoPadrao = "Filtro de oleo";
    public const TipoPecaInsumo TipoPadrao = TipoPecaInsumo.PECA;
    public const decimal ValorPadrao = 45m;

    public const string DescricaoAtualizada = "Oleo 5W30";
    public const TipoPecaInsumo TipoAtualizado = TipoPecaInsumo.INSUMO;
    public const decimal ValorAtualizado = 38m;

    public const int PaginaPadrao = 1;
    public const int TamanhoPaginaPadrao = 10;

    public static PecaInsumoCatalogo CriarPecaInsumoCatalogoPadrao(string descricao = DescricaoPadrao, TipoPecaInsumo tipo = TipoPadrao, decimal valor = ValorPadrao)
    {
        return PecaInsumoCatalogo.Criar(descricao, tipo, valor);
    }

    public static CadastrarPecaInsumoCatalogoRequest CriarCadastrarPecaInsumoCatalogoRequestValido(string descricao = DescricaoPadrao, TipoPecaInsumo tipo = TipoPadrao, decimal valor = ValorPadrao)
    {
        return new CadastrarPecaInsumoCatalogoRequest(descricao, tipo, valor);
    }

    public static AtualizarPecaInsumoCatalogoRequest CriarAtualizarPecaInsumoCatalogoRequestValido(
        Guid? pecaInsumoCatalogoId = null,
        string descricao = DescricaoAtualizada,
        TipoPecaInsumo tipo = TipoAtualizado,
        decimal valor = ValorAtualizado)
    {
        return new AtualizarPecaInsumoCatalogoRequest(pecaInsumoCatalogoId ?? Guid.NewGuid(), descricao, tipo, valor);
    }

    public static ConsultarPecaInsumoCatalogoRequest CriarConsultarPecaInsumoCatalogoRequestValido(Guid? pecaInsumoCatalogoId = null)
    {
        return new ConsultarPecaInsumoCatalogoRequest(pecaInsumoCatalogoId ?? Guid.NewGuid());
    }

    public static ListarPecasInsumosCatalogoRequest CriarListarPecasInsumosCatalogoRequestValido(int pagina = PaginaPadrao, int tamanhoPagina = TamanhoPaginaPadrao)
    {
        return new ListarPecasInsumosCatalogoRequest(pagina, tamanhoPagina);
    }

    public static RemoverPecaInsumoCatalogoRequest CriarRemoverPecaInsumoCatalogoRequestValido(Guid? pecaInsumoCatalogoId = null)
    {
        return new RemoverPecaInsumoCatalogoRequest(pecaInsumoCatalogoId ?? Guid.NewGuid());
    }
}
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.AtualizarPecaInsumoCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;
using OficinaMecanica.Domain.Administrativo.Enums;

namespace OficinaMecanica.API.IntegrationTests.Administrativo.Builders;

public sealed class PecaInsumoCatalogoRequestBuilder
{
    private string _descricao = "Filtro de oleo";
    private TipoPecaInsumo _tipo = TipoPecaInsumo.PECA;
    private decimal _valor = 45m;

    public static PecaInsumoCatalogoRequestBuilder Novo()
    {
        return new PecaInsumoCatalogoRequestBuilder();
    }

    public PecaInsumoCatalogoRequestBuilder ComDescricao(string descricao)
    {
        _descricao = descricao;

        return this;
    }

    public PecaInsumoCatalogoRequestBuilder ComTipo(TipoPecaInsumo tipo)
    {
        _tipo = tipo;

        return this;
    }

    public PecaInsumoCatalogoRequestBuilder ComValor(decimal valor)
    {
        _valor = valor;

        return this;
    }

    public CadastrarPecaInsumoCatalogoRequest BuildCadastro()
    {
        return new CadastrarPecaInsumoCatalogoRequest(_descricao, _tipo, _valor);
    }

    public AtualizarPecaInsumoCatalogoRequest BuildAtualizacao(Guid pecaInsumoCatalogoId)
    {
        return new AtualizarPecaInsumoCatalogoRequest(pecaInsumoCatalogoId, _descricao, _tipo, _valor);
    }
}

using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.AtualizarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.CadastrarServicoCatalogo;

namespace OficinaMecanica.API.IntegrationTests.Administrativo.Builders;

public sealed class ServicoCatalogoRequestBuilder
{
    private string _descricao = "Troca de oleo";
    private decimal _valor = 150m;

    public static ServicoCatalogoRequestBuilder Novo()
    {
        return new ServicoCatalogoRequestBuilder();
    }

    public ServicoCatalogoRequestBuilder ComDescricao(string descricao)
    {
        _descricao = descricao;

        return this;
    }

    public ServicoCatalogoRequestBuilder ComValor(decimal valor)
    {
        _valor = valor;

        return this;
    }

    public CadastrarServicoCatalogoRequest BuildCadastro()
    {
        return new CadastrarServicoCatalogoRequest(_descricao, _valor);
    }

    public AtualizarServicoCatalogoRequest BuildAtualizacao(Guid servicoCatalogoId)
    {
        return new AtualizarServicoCatalogoRequest(servicoCatalogoId, _descricao, _valor);
    }
}

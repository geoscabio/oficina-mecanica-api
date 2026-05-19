using OficinaMecanica.Application.Administrativo.MecanicoUseCases.AtualizarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.CadastrarMecanico;

namespace OficinaMecanica.API.IntegrationTests.Administrativo.Builders;

public sealed class MecanicoRequestBuilder
{
    private string _nome = "Joao Mecanico";
    private string _funcional = "MEC001";

    public static MecanicoRequestBuilder Novo()
    {
        return new MecanicoRequestBuilder();
    }

    public MecanicoRequestBuilder ComNome(string nome)
    {
        _nome = nome;

        return this;
    }

    public MecanicoRequestBuilder ComFuncional(string funcional)
    {
        _funcional = funcional;

        return this;
    }

    public CadastrarMecanicoRequest BuildCadastro()
    {
        return new CadastrarMecanicoRequest(_nome, _funcional);
    }

    public AtualizarMecanicoRequest BuildAtualizacao(Guid mecanicoId)
    {
        return new AtualizarMecanicoRequest(mecanicoId, _nome, _funcional);
    }
}

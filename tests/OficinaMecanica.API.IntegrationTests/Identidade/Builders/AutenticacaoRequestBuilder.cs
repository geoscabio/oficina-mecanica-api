using OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;

namespace OficinaMecanica.API.IntegrationTests.Identidade.Builders;

public sealed class AutenticacaoRequestBuilder
{
    private string _login = "admin";
    private string _senha = "admin123";

    public static AutenticacaoRequestBuilder Novo()
    {
        return new AutenticacaoRequestBuilder();
    }

    public AutenticacaoRequestBuilder ComLogin(string login)
    {
        _login = login;

        return this;
    }

    public AutenticacaoRequestBuilder ComSenha(string senha)
    {
        _senha = senha;

        return this;
    }

    public AutenticarUsuarioRequest Build()
    {
        return new AutenticarUsuarioRequest(_login, _senha);
    }
}

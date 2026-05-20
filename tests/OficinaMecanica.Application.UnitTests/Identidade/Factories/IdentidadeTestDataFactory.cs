using OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;

namespace OficinaMecanica.Application.UnitTests.Identidade.Factories;

internal static class IdentidadeTestDataFactory
{
    public const string TokenPadrao = "token-jwt";
    public const string LoginPadrao = "admin";
    public const string SenhaPadrao = "admin123";
    public const string NomePadrao = "Administrador";
    public const string PerfilPadrao = "Administrador";

    public static AutenticarUsuarioRequest CriarAutenticarUsuarioRequestValido(string login = LoginPadrao, string senha = SenhaPadrao)
    {
        return new AutenticarUsuarioRequest(login, senha);
    }

    public static AutenticarUsuarioResponse CriarAutenticarUsuarioResponseValido(string token = "")
    {
        return new AutenticarUsuarioResponse(token, Guid.NewGuid(), NomePadrao, LoginPadrao, PerfilPadrao);
    }
}
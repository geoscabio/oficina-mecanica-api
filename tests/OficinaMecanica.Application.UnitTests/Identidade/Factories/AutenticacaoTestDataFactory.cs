using OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;

namespace OficinaMecanica.Application.UnitTests.Identidade.Factories;

public static class AutenticacaoTestDataFactory
{
    public static AutenticarUsuarioRequest CriarRequestAutenticacaoValida()
    {
        return new AutenticarUsuarioRequest(
            "admin",
            "admin123");
    }

    public static AutenticarUsuarioResponse CriarUsuarioAutenticadoPadrao()
    {
        return new AutenticarUsuarioResponse(
            string.Empty,
            Guid.NewGuid(),
            "Administrador",
            "admin",
            "Administrador");
    }
}
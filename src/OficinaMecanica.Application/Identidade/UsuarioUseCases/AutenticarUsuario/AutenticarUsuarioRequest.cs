namespace OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;

public sealed record AutenticarUsuarioRequest(
    string Login,
    string Senha);
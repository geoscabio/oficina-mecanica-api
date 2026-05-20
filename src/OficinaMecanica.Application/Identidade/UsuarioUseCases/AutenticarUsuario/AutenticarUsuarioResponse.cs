namespace OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;

public sealed record AutenticarUsuarioResponse(string Token, Guid UsuarioId, string Nome, string Login, string Perfil);
using OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;

namespace OficinaMecanica.Application.Identidade.Interfaces;

public interface IUsuarioAutenticadoService
{
    Task<AutenticarUsuarioResponse?> AutenticarAsync(
        string login,
        string senha,
        CancellationToken cancellationToken = default);
}
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using OficinaMecanica.Application.Identidade.Interfaces;
using OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;
using OficinaMecanica.Infrastructure.Identidade.Options;

namespace OficinaMecanica.Infrastructure.Identidade.Services;

public sealed class UsuarioDemoAutenticadoService : IUsuarioAutenticadoService
{
    private readonly IdentidadeOptions _options;

    public UsuarioDemoAutenticadoService(IOptions<IdentidadeOptions> options)
    {
        _options = options.Value;
    }

    public Task<AutenticarUsuarioResponse?> AutenticarAsync(string login, string senha, CancellationToken cancellationToken = default)
    {
        var usuarioDemo = ObterUsuariosDemo()
            .FirstOrDefault(usuario =>
                string.Equals(login, usuario.Login, StringComparison.OrdinalIgnoreCase)
                && senha == usuario.Senha);

        if (usuarioDemo is null)
        {
            return Task.FromResult<AutenticarUsuarioResponse?>(null);
        }

        return Task.FromResult<AutenticarUsuarioResponse?>(new AutenticarUsuarioResponse(Token: string.Empty, UsuarioId: usuarioDemo.UsuarioId, Nome: usuarioDemo.Nome, Login: usuarioDemo.Login, Perfil: usuarioDemo.Perfil));
    }

    private IEnumerable<UsuarioDemo> ObterUsuariosDemo()
    {
        return _options.UsuariosDemo
            .Select(usuario => CriarUsuarioDemo(usuario))
            .Where(usuario => usuario is not null)
            .Cast<UsuarioDemo>();
    }

    private static UsuarioDemo? CriarUsuarioDemo(UsuarioDemoOptions usuario)
    {
        if (!usuario.EhValido())
        {
            return null;
        }

        return new UsuarioDemo(GerarUsuarioId(usuario.Login), usuario.Nome, usuario.Login, usuario.Senha, usuario.Perfil);
    }

    private static Guid GerarUsuarioId(string login)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(login.Trim().ToLowerInvariant()));

        return new Guid(hash.AsSpan(0, 16));
    }

    private sealed record UsuarioDemo(Guid UsuarioId, string Nome, string Login, string Senha, string Perfil);
}

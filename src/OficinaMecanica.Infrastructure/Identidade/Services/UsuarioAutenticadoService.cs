using Microsoft.Extensions.Configuration;
using OficinaMecanica.Application.Identidade.Interfaces;
using OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;

namespace OficinaMecanica.Infrastructure.Identidade.Services;

public sealed class UsuarioAutenticadoService : IUsuarioAutenticadoService
{
    private readonly IConfiguration _configuration;

    public UsuarioAutenticadoService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<AutenticarUsuarioResponse?> AutenticarAsync(
        string login,
        string senha,
        CancellationToken cancellationToken = default)
    {
        var usuarioDemo = ObterUsuarioDemo();

        if (!string.Equals(login, usuarioDemo.Login, StringComparison.OrdinalIgnoreCase)
            || senha != usuarioDemo.Senha)
        {
            return Task.FromResult<AutenticarUsuarioResponse?>(null);
        }

        return Task.FromResult<AutenticarUsuarioResponse?>(
            new AutenticarUsuarioResponse(
                Token: string.Empty,
                UsuarioId: usuarioDemo.UsuarioId,
                Nome: usuarioDemo.Nome,
                Login: usuarioDemo.Login,
                Perfil: usuarioDemo.Perfil));
    }

    private UsuarioDemo ObterUsuarioDemo()
    {
        return new UsuarioDemo(
            UsuarioId: Guid.NewGuid(),
            Nome: _configuration["Identidade:UsuarioDemo:Nome"] ?? "Administrador",
            Login: _configuration["Identidade:UsuarioDemo:Login"] ?? "admin",
            Senha: _configuration["Identidade:UsuarioDemo:Senha"] ?? "admin123",
            Perfil: _configuration["Identidade:UsuarioDemo:Perfil"] ?? "Administrador");
    }

    private sealed record UsuarioDemo(
        Guid UsuarioId,
        string Nome,
        string Login,
        string Senha,
        string Perfil);
}

using System.Text;
using OficinaMecanica.Application.Identidade.Interfaces;

namespace OficinaMecanica.Infrastructure.Identidade.Services;

public sealed class TokenService : ITokenService
{
    public string GerarToken(
        Guid usuarioId,
        string nome,
        string login,
        string perfil)
    {
        var token = string.Join(
            "|",
            usuarioId,
            nome,
            login,
            perfil,
            DateTimeOffset.UtcNow.ToString("O"));

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
    }
}

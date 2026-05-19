using System.Text;

namespace OficinaMecanica.Infrastructure.Identidade.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Secret { get; init; } = string.Empty;
    public int ExpirationMinutes { get; init; } = 120;

    public bool EhValido()
    {
        return !string.IsNullOrWhiteSpace(Issuer)
            && !string.IsNullOrWhiteSpace(Audience)
            && !string.IsNullOrWhiteSpace(Secret)
            && Encoding.UTF8.GetByteCount(Secret) >= 32
            && ExpirationMinutes > 0;
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Issuer))
        {
            throw new InvalidOperationException("Jwt:Issuer nao configurado.");
        }

        if (string.IsNullOrWhiteSpace(Audience))
        {
            throw new InvalidOperationException("Jwt:Audience nao configurado.");
        }

        if (string.IsNullOrWhiteSpace(Secret))
        {
            throw new InvalidOperationException("Jwt:Secret nao configurado.");
        }

        if (Encoding.UTF8.GetByteCount(Secret) < 32)
        {
            throw new InvalidOperationException("Jwt:Secret deve ter pelo menos 32 caracteres.");
        }
    }
}

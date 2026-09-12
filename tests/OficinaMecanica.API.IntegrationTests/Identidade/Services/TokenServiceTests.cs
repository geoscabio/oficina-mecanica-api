using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Options;
using OficinaMecanica.Infrastructure.Identidade.Options;
using OficinaMecanica.Infrastructure.Identidade.Services;

namespace OficinaMecanica.API.IntegrationTests.Identidade.Services;

public sealed class TokenServiceTests
{
    [Fact]
    public void Dado_UsuarioInterno_Quando_GerarToken_Entao_DeveRespeitarContratoJwtBase()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var antesDaEmissao = DateTime.UtcNow;
        var service = CriarService();

        // Act
        var token = service.GerarToken(usuarioId, "Administrador", "admin", "Administrador");

        // Assert
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Header.Alg.Should().Be("HS256");
        jwt.Issuer.Should().Be("oficina-mecanica-auth");
        jwt.Audiences.Should().ContainSingle().Which.Should().Be("oficina-mecanica-api");
        jwt.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.Sub && claim.Value == usuarioId.ToString());
        jwt.Claims.Should().Contain(claim => claim.Type == "role" && claim.Value == "Administrador");
        jwt.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.Jti && EhGuid(claim.Value));
        jwt.ValidTo.Should().BeCloseTo(antesDaEmissao.AddMinutes(60), TimeSpan.FromSeconds(5));
        jwt.Claims.Should().NotContain(claim => EhClaimDeDocumentoOuClienteId(claim.Type));
    }

    private static TokenService CriarService()
    {
        return new TokenService(
            Options.Create(
                new JwtOptions
                {
                    Issuer = "oficina-mecanica-auth",
                    Audience = "oficina-mecanica-api",
                    Secret = "oficina-mecanica-api-chave-academica-jwt-2026",
                    ExpirationMinutes = 60
                }));
    }

    private static bool EhClaimDeDocumentoOuClienteId(string claimType)
    {
        return claimType == "cliente_id"
            || claimType == "cpf"
            || claimType == "cnpj"
            || claimType == "cpf_hash"
            || claimType == "cnpj_hash"
            || claimType == "documento_hash";
    }

    private static bool EhGuid(string value)
    {
        return Guid.TryParse(value, out _);
    }
}

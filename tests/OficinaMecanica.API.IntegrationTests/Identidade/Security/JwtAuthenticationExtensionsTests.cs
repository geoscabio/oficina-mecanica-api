using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OficinaMecanica.API.Extensions.Security;

namespace OficinaMecanica.API.IntegrationTests.Identidade.Security;

public sealed class JwtAuthenticationExtensionsTests
{
    private const string Issuer = "oficina-mecanica-auth";
    private const string Audience = "oficina-mecanica-api";
    private const string Secret = "oficina-mecanica-api-chave-academica-jwt-2026";

    [Fact]
    public void Dado_TokenFuturoDeClienteValido_Quando_ValidarJwt_Entao_DeveSerAceito()
    {
        // Arrange
        var clienteId = Guid.NewGuid().ToString();
        var options = CriarOptions();
        var token = CriarToken(
            Issuer,
            Audience,
            Secret,
            DateTime.UtcNow.AddMinutes(60),
            [
                new Claim(JwtRegisteredClaimNames.Sub, clienteId),
                new Claim("role", "Cliente"),
                new Claim("cliente_id", clienteId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ]);

        // Act
        var principal = Validar(token, options);

        // Assert
        principal.Identity!.IsAuthenticated.Should().BeTrue();
        principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value.Should().Be(clienteId);
        principal.FindFirst("role")!.Value.Should().Be("Cliente");
        principal.FindFirst("cliente_id")!.Value.Should().Be(clienteId);
    }

    [Theory]
    [InlineData("emissor-incorreto", Audience, Secret, false)]
    [InlineData(Issuer, "audiencia-incorreta", Secret, false)]
    [InlineData(Issuer, Audience, "assinatura-incorreta-chave-jwt-2026", false)]
    [InlineData(Issuer, Audience, Secret, true)]
    public void Dado_TokenInvalido_Quando_ValidarJwt_Entao_DeveSerRejeitado(
        string issuer,
        string audience,
        string secret,
        bool expirado)
    {
        // Arrange
        var token = CriarToken(
            issuer,
            audience,
            secret,
            expirado ? DateTime.UtcNow.AddMinutes(-1) : DateTime.UtcNow.AddMinutes(60),
            [
                new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()),
                new Claim("role", "Administrador"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ]);

        // Act
        Action act = () => Validar(token, CriarOptions());

        // Assert
        act.Should().Throw<SecurityTokenException>();
    }

    private static JwtBearerOptions CriarOptions()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Jwt:Issuer"] = Issuer,
                    ["Jwt:Audience"] = Audience,
                    ["Jwt:Secret"] = Secret,
                    ["Jwt:ExpirationMinutes"] = "60"
                })
            .Build();

        var services = new ServiceCollection();
        services.AddJwtAuthentication(configuration);

        using var serviceProvider = services.BuildServiceProvider();

        return serviceProvider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);
    }

    private static ClaimsPrincipal Validar(string token, JwtBearerOptions options)
    {
        var handler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = options.MapInboundClaims
        };

        return handler
            .ValidateToken(token, options.TokenValidationParameters, out _);
    }

    private static string CriarToken(
        string issuer,
        string audience,
        string secret,
        DateTime expires,
        IEnumerable<Claim> claims)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);

        return new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expires,
                signingCredentials: credentials));
    }
}

using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Options;
using OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;
using OficinaMecanica.Infrastructure.Identidade.Options;
using OficinaMecanica.Infrastructure.Identidade.Services;

namespace OficinaMecanica.API.IntegrationTests.Identidade.Services;

public sealed class AutenticacaoInternaJwtTests
{
    [Theory]
    [InlineData("admin", "admin123", "Administrador")]
    [InlineData("atendente", "atendente123", "Atendente")]
    [InlineData("mecanico", "mecanico123", "Mecanico")]
    [InlineData("cliente", "cliente123", "Cliente")]
    public async Task Dado_UsuarioDemo_Quando_Autenticar_Entao_DeveManterLoginInternoComContratoJwtBase(
        string login,
        string senha,
        string perfil)
    {
        // Arrange
        var useCase = new AutenticarUsuarioUseCase(
            new UsuarioDemoAutenticadoService(Options.Create(CriarIdentidadeOptions())),
            new TokenService(Options.Create(CriarJwtOptions())),
            new AutenticarUsuarioValidator());

        // Act
        var resultado = await useCase.ExecuteAsync(new AutenticarUsuarioRequest(login, senha));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor!.Perfil.Should().Be(perfil);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(resultado.Valor.Token);

        jwt.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.Sub && EhGuid(claim.Value));
        jwt.Claims.Should().Contain(claim => claim.Type == "role" && claim.Value == perfil);
        jwt.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.Jti && EhGuid(claim.Value));
        jwt.Claims.Should().NotContain(claim => EhClaimDeDocumento(claim.Type));

        if (perfil == "Cliente")
        {
            jwt.Claims.Should().NotContain(claim => claim.Type == "cliente_id");
        }
    }

    private static IdentidadeOptions CriarIdentidadeOptions()
    {
        return new IdentidadeOptions
        {
            UsuariosDemo =
            [
                new UsuarioDemoOptions { Nome = "Administrador", Login = "admin", Senha = "admin123", Perfil = "Administrador" },
                new UsuarioDemoOptions { Nome = "Atendente", Login = "atendente", Senha = "atendente123", Perfil = "Atendente" },
                new UsuarioDemoOptions { Nome = "Mecanico", Login = "mecanico", Senha = "mecanico123", Perfil = "Mecanico" },
                new UsuarioDemoOptions { Nome = "Cliente", Login = "cliente", Senha = "cliente123", Perfil = "Cliente" }
            ]
        };
    }

    private static JwtOptions CriarJwtOptions()
    {
        return new JwtOptions
        {
            Issuer = "oficina-mecanica-auth",
            Audience = "oficina-mecanica-api",
            Secret = "oficina-mecanica-api-chave-academica-jwt-2026",
            ExpirationMinutes = 60
        };
    }

    private static bool EhClaimDeDocumento(string claimType)
    {
        return claimType == "cpf"
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

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace OficinaMecanica.API.IntegrationTests.Fixtures;

public sealed class OficinaMecanicaApiFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(CriarConfiguracaoTeste());
        });

        builder.UseSetting("Database:ApplyMigrationsOnStartup", "false");
        builder.UseSetting("Database:SeedDemoData", "false");
    }

    private static IReadOnlyDictionary<string, string?> CriarConfiguracaoTeste()
    {
        return new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "OficinaMecanica",
            ["Jwt:Audience"] = "OficinaMecanica.API",
            ["Jwt:Secret"] = "oficina-mecanica-api-chave-academica-jwt-2026",
            ["Jwt:ExpirationMinutes"] = "120",
            ["Identidade:UsuariosDemo:0:Nome"] = "Administrador",
            ["Identidade:UsuariosDemo:0:Login"] = "admin",
            ["Identidade:UsuariosDemo:0:Senha"] = "admin123",
            ["Identidade:UsuariosDemo:0:Perfil"] = "Administrador",
            ["Identidade:UsuariosDemo:1:Nome"] = "Cliente",
            ["Identidade:UsuariosDemo:1:Login"] = "cliente",
            ["Identidade:UsuariosDemo:1:Senha"] = "cliente123",
            ["Identidade:UsuariosDemo:1:Perfil"] = "Cliente"
        };
    }
}

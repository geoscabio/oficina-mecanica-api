using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Infrastructure.Persistence;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;

namespace OficinaMecanica.API.IntegrationTests.Fixtures;

public sealed class OficinaMecanicaApiFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
    private readonly Dictionary<string, string?> _variaveisAmbienteOriginais = [];
    private Respawner? _respawner;
    private WebApplicationFactory<Program>? _factory;
    private string? _connectionString;

    public HttpClient CreateClient()
    {
        return Factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _sqlServer.StartAsync();

        _connectionString = _sqlServer.GetConnectionString();
        ConfigurarVariaveisAmbienteTeste(_connectionString);

        _factory = new OficinaMecanicaWebApplicationFactory();

        await AplicarMigrationsAsync();
        await ConfigurarRespawnAsync();
    }

    public async Task ResetarBancoAsync()
    {
        if (_respawner is null || _connectionString is null)
        {
            throw new InvalidOperationException("Banco de testes nao foi inicializado.");
        }

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public async Task DisposeAsync()
    {
        _factory?.Dispose();
        await _sqlServer.DisposeAsync();
        RestaurarVariaveisAmbiente();
    }

    private WebApplicationFactory<Program> Factory =>
        _factory ?? throw new InvalidOperationException("Factory de API nao foi inicializada.");

    private async Task AplicarMigrationsAsync()
    {
        await using var dbContext = CriarDbContext();

        await dbContext.Database.MigrateAsync();
    }

    private async Task ConfigurarRespawnAsync()
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                SchemasToInclude =
                [
                    "Administrativo",
                    "Atendimento",
                    "GestaoEstoque",
                    "GestaoOrdemServico"
                ],
                TablesToIgnore =
                [
                    new Table("__EFMigrationsHistory")
                ]
            });
    }

    private OficinaMecanicaDbContext CriarDbContext()
    {
        var options = new DbContextOptionsBuilder<OficinaMecanicaDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        return new OficinaMecanicaDbContext(options);
    }

    private void ConfigurarVariaveisAmbienteTeste(string connectionString)
    {
        foreach (var item in CriarConfiguracaoTeste(connectionString))
        {
            _variaveisAmbienteOriginais[item.Key] = Environment.GetEnvironmentVariable(item.Key);
            Environment.SetEnvironmentVariable(item.Key, item.Value);
        }
    }

    private void RestaurarVariaveisAmbiente()
    {
        foreach (var item in _variaveisAmbienteOriginais)
        {
            Environment.SetEnvironmentVariable(item.Key, item.Value);
        }
    }

    private static Dictionary<string, string?> CriarConfiguracaoTeste(string connectionString)
    {
        return new Dictionary<string, string?>
        {
            ["ConnectionStrings__DefaultConnection"] = connectionString,
            ["Database__ApplyMigrationsOnStartup"] = "false",
            ["Database__SeedDemoData"] = "false",
            ["Jwt__Issuer"] = "OficinaMecanica",
            ["Jwt__Audience"] = "OficinaMecanica.API",
            ["Jwt__Secret"] = "oficina-mecanica-api-chave-academica-jwt-2026",
            ["Jwt__ExpirationMinutes"] = "120",
            ["Integracoes__Orcamento__WebhookToken"] = "webhook-orcamento-teste-local-2026",
            ["Identidade__UsuariosDemo__0__Nome"] = "Administrador",
            ["Identidade__UsuariosDemo__0__Login"] = "admin",
            ["Identidade__UsuariosDemo__0__Senha"] = "admin123",
            ["Identidade__UsuariosDemo__0__Perfil"] = "Administrador",
            ["Identidade__UsuariosDemo__1__Nome"] = "Atendente",
            ["Identidade__UsuariosDemo__1__Login"] = "atendente",
            ["Identidade__UsuariosDemo__1__Senha"] = "atendente123",
            ["Identidade__UsuariosDemo__1__Perfil"] = "Atendente",
            ["Identidade__UsuariosDemo__2__Nome"] = "Mecanico",
            ["Identidade__UsuariosDemo__2__Login"] = "mecanico",
            ["Identidade__UsuariosDemo__2__Senha"] = "mecanico123",
            ["Identidade__UsuariosDemo__2__Perfil"] = "Mecanico",
            ["Identidade__UsuariosDemo__3__Nome"] = "Cliente",
            ["Identidade__UsuariosDemo__3__Login"] = "cliente",
            ["Identidade__UsuariosDemo__3__Senha"] = "cliente123",
            ["Identidade__UsuariosDemo__3__Perfil"] = "Cliente"
        };
    }

    private sealed class OficinaMecanicaWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }
    }
}

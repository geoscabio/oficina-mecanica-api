using OficinaMecanica.API.Extensions.Configuration;
using OficinaMecanica.API.Extensions.Middlewares;
using OficinaMecanica.Application;
using OficinaMecanica.Infrastructure;
using OficinaMecanica.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

var startupLogger = app.Services
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("OficinaMecanica.Startup");

startupLogger.LogInformation("Inicializando OficinaMecanica API...");
await app.Services.InitializeDatabaseAsync(builder.Configuration);
startupLogger.LogInformation("API pronta. Swagger disponivel em /swagger.");

app.UseApiMiddlewares();
app.MapApiEndpoints();

await app.RunAsync();

#pragma warning disable ASP0027
// Public Program keeps WebApplicationFactory<Program> available to integration tests.
public partial class Program
{
    protected Program()
    {
    }
}
#pragma warning restore ASP0027

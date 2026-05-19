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

app.UseApiPipeline();
app.MapApiEndpoints();

app.Run();

public partial class Program;

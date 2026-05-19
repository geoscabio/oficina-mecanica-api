using OficinaMecanica.Application;
using OficinaMecanica.Infrastructure;
using OficinaMecanica.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.Services.InitializeDatabaseAsync(builder.Configuration);

app.UseApiPipeline();
app.MapApiEndpoints();

app.Run();

public partial class Program;

using OficinaMecanica.Infrastructure;
using OficinaMecanica.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.Services.InitializeDatabaseAsync(builder.Configuration);

app.UseHttpsRedirection();

app.Run();
